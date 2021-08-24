using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraper
{
    class Scraper
    {
        public static List<Review> reviews_scraped = new List<Review>();
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            //get reviews for 5 pages - this could be customised easily to change page numbers
            for (int i = 1; i <= 5; i++)
            {
                tasks.Add(GetHtmlAsync(i.ToString()));
            }
            Task.WaitAll(tasks.ToArray());

            //calculate the score based on friendliness, pricing etc
            calculate_score();

            //sort based on O(Log n) algorithm to find the top 3 scores and log it to console...
            List<Review> top_three = final_top_three();

            for (int i = 1; i <= top_three.Count; i++)
            {
                Console.WriteLine("Highest Score " + i + " : " + top_three[i - 1].score);
                Console.WriteLine("Author " + i + " : " + top_three[i - 1].author);
                Console.WriteLine("Review Body " + i + " : " + top_three[i - 1].review_body);
            }

            Console.ReadLine();
        }

        //uses the html agility to scrape the first 5 pages - parses the html and stores the value in a list
        private static async Task GetHtmlAsync(string page)
        {
            var url = "https://www.dealerrater.com/dealer/McKaig-Chevrolet-Buick-A-Dealer-For-The-People-dealer-reviews-23685/page" + page;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            HtmlNodeCollection reviewBody = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'review-entry')]");

            if (reviewBody != null)
            {
                for (int i = 0; i < reviewBody.Count; i++)
                {
                    string author = reviewBody[i].SelectNodes("//span[contains(@class, 'notranslate') and contains(@class, 'italic') and contains(@class, 'font-18') and contains(@class, 'black')]")[i].InnerText.Replace('-', ' ').Trim();
                    string review_body = reviewBody[i].SelectNodes("//p[contains(@class, 'font-16') and contains(@class, 'review-content') and contains(@class, 'margin-bottom-none') and contains(@class, 'line-height-25')]")[i].InnerText.Trim();
                    string overall_rating = getRatingFromHtml(reviewBody[i].SelectNodes("//div[contains(@class, 'margin-none') and contains(@class, 'pad-none') and contains(@class, 'rating-static') and starts-with(@class, 'rating-')]")[i].OuterHtml.Trim());

                    var test = reviewBody[i].ChildNodes;
                    var tt = test.Descendants("div").Where(x => x.InnerText.Equals("Pricing")).FirstOrDefault();
                    var tgg = tt != null ? tt.ParentNode.OuterHtml : "";

                    HtmlNode cust_service = reviewBody[i].ChildNodes.Descendants("div").Where(x => x.InnerText.Equals("Customer Service")).FirstOrDefault();
                    string customer_service = (cust_service != null) ? getRatingFromHtml(cust_service.ParentNode.OuterHtml) : "0";

                    HtmlNode friendliness_parent = reviewBody[i].ChildNodes.Descendants("div").Where(x => x.InnerText.Equals("Friendliness")).FirstOrDefault();
                    string friendliness = (friendliness_parent != null) ? getRatingFromHtml(friendliness_parent.ParentNode.OuterHtml) : "0";

                    HtmlNode pricing_parent = reviewBody[i].ChildNodes.Descendants("div").Where(x => x.InnerText.Equals("Pricing")).FirstOrDefault();
                    string pricing = (pricing_parent != null) ? getRatingFromHtml(pricing_parent.ParentNode.OuterHtml) : "0";

                    HtmlNode overall_experience_parent = reviewBody[i].ChildNodes.Descendants("div").Where(x => x.InnerText.Equals("Overall Experience")).FirstOrDefault();
                    string overall_experience = (overall_experience_parent != null) ? getRatingFromHtml(overall_experience_parent.ParentNode.OuterHtml) : "0";

                    HtmlNode recommend_dealer_parent = reviewBody[i].ChildNodes.Descendants("div").Where(x => x.InnerText.Equals("Recommend Dealer")).FirstOrDefault();
                    string recommend_dealer = (recommend_dealer_parent != null) ? recommend_dealer_parent.ParentNode.ChildNodes[3].InnerText.Replace("/r/n", " ").Trim() : "0";

                    Review r = new Review();

                    r.author = author;
                    r.review_body = string.IsNullOrEmpty(review_body) ? " " : review_body;
                    r.customer_service = string.IsNullOrEmpty(customer_service) ? 0.ToString() : customer_service;
                    r.overall_rating = string.IsNullOrEmpty(overall_rating) ? 0.ToString() : overall_rating;
                    r.friendliness = string.IsNullOrEmpty(friendliness) ? 0.ToString() : friendliness; ;
                    r.pricing = string.IsNullOrEmpty(pricing) ? 0.ToString() : pricing; ;
                    r.overall_experience = string.IsNullOrEmpty(overall_experience) ? 0.ToString() : overall_experience; ;
                    r.recommend_dealer = string.IsNullOrEmpty(recommend_dealer) ? " " : recommend_dealer; ;

                    reviews_scraped.Add(r);
                }

            }
        }


        //parses outer html and gets the value of the rating by the css class
        private static string getRatingFromHtml(string outerHtml)
        {
            if (string.IsNullOrEmpty(outerHtml))
                return "";
            string[] splitStr = outerHtml.Split(' ');

            foreach (string s in splitStr)
            {
                if (s.StartsWith("rating-") && !s.Contains("rating-static"))
                {
                    return s.Substring(7);
                }
            }
            return " ";
        }

        private static void calculate_score()
        {
            foreach (Review review in reviews_scraped)
            {
               //we can calculate the score by a sentiment analyser for the review boday as well - I have not done it here but thats a good one to do too!
                double score = 0;              
                score += Convert.ToDouble(review.overall_rating) + Convert.ToDouble(review.pricing) + Convert.ToDouble(review.friendliness) + Convert.ToDouble(review.customer_service) + Convert.ToDouble(review.overall_experience);

                if (review.recommend_dealer.ToLower().Contains("yes"))
                {
                    score += 10;
                }

                review.score = Math.Round(score, 2);

            }
        }

        private static List<Review> final_top_three()
        {
            return reviews_scraped.OrderByDescending(x => x.score).Take(3).ToList();
        }
    }
}
