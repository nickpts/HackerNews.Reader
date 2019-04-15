## HackerNews.Reader

#### Description
A simple console app that outputs the top posts in Hacker News in json format.

#### Implementation
HN hosts a Rest API as seen here: https://github.com/HackerNews/API. For brevity, and conciseness, this was the best approach to pull information out of HN. 

Other approaches considered:
- Web scraping: slow and inefficient. HtmlAgilityPack is needed (unless a scraper is written from scratch) and even that is cumbersome as it creates an XDocument behind the scenes where it loads all the content pages as XElements and XNodes. Xpath is needed to navigate efficiently. Compared with memory requirements this makes running and debugging slow. Not the most readable code either.
- IronWebScraper: newer third-party library which is more lightweight than HtmlAgilityPack, unfortunately not available for .NET Core.
- Algolia API: older HN web API. Clunky with limited calls available. 

#### Scope
The code is as consise as it could be made while fullfilling the spec. Out of scope at this point were interfaces and dependency injection. These increase modularity and reduce maintenance and enable mocking in unit testing. For the size of the project, I decided to use concrete classes only.

Unit tests cover all input validation and main functionality routes.

#### How to run
The following third party libraries are needed:
1) Newtonsoft.Json (for serialization/deserialization)
2) FluentAssertions (for unit testing)

A Dockerfile is included, the base images specified are dotnet runtime and dotnet sdk. These need to be installed along with docker ce.

Given the above are installed

1) Clone this repository in a folder of your choosing
2) Manually move the Dockerfile HackerNews.Reader/HackerNews.Reader to HackerNews.Reader (the root directory). This is a well-know bug that VS2017 introduces when building a Dockerfile.
3) In HackerNews.Reader build the image
  docker build.
4) When you have the image run a container as follows:
  docker run --entrypoint="dotnet" [image] HackerNews.Reader.dll hackernews --posts 100


