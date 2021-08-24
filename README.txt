Coding Challenge: “A Dealer For the People”
The KGB has noticed a resurgence of overly excited reviews for a McKaig Chevrolet Buick, a dealership they have planted in the United States. In order to avoid attracting unwanted attention, you’ve been enlisted to scrape reviews for this dealership from DealerRater.com and uncover the top three worst offenders of these overly positive endorsements.

Your mission, should you choose to accept it, is to write a tool that:

scrapes the first five pages of reviews
identifies the top three most “overly positive” endorsements (using criteria of your choosing, documented in the README)
outputs these three reviews to the console, in order of severity
Review Score Calculation
Using the star ratings and sentiment analysis, each review gets a final score out of 50.

Overall Star Rating - 0 to 50
Customer Service - 0 to 50
Quality Of Work - 0 to 50
Friendliness - 0 to 50
Pricing - 0 to 50
Experience - 0 to 50
Recommend Dealer - Yes(+10) or No(+0)
Sentiment analysis on review body - 0 to 10 (+/-) ( This is nice to have - Couldnt do this this time!)


This is a.Net Console application 
Please install HTMLAgility Nuget

This scrapes directly from the URL https://www.dealerrater.com/dealer/McKaig-Chevrolet-Buick-A-Dealer-For-The-People-dealer-reviews-23685