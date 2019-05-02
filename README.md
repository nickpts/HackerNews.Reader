## HackerNews.Reader

#### Description
A .NET standard library that that retrieves HackerNews posts in either POCO or JSON format.

#### Implementation
HN hosts a Rest API as seen here: https://github.com/HackerNews/API. For brevity, and conciseness, this was the best approach to pull information out of HN. 

Other approaches considered:
- Web scraping: slow and inefficient. HtmlAgilityPack is needed (unless a scraper is written from scratch) and even that is cumbersome as it creates an XDocument behind the scenes where it loads all the content pages as XElements and XNodes. Xpath is needed to navigate efficiently. Compared with memory requirements this makes running and debugging slow. Not the most readable code either.
- IronWebScraper: newer third-party library which is more lightweight than HtmlAgilityPack, unfortunately not available for .NET Core.
- Algolia API: the API HN uses every time you make a search through the website. Clunky with limited calls available, I found the data to be sometimes unreliable.

#### How to run
The following third party libraries are needed:
1) Newtonsoft.Json (for serialization/deserialization)
2) FluentAssertions (for unit testing)



