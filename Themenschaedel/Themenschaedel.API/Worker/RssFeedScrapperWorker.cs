using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using FluentScheduler;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Worker
{
    public class RssFeedScrapperWorker : BackgroundService
    {
        private readonly ILogger<RssFeedScrapperWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabaseService _database;

        public RssFeedScrapperWorker(ILogger<RssFeedScrapperWorker> logger, IConfiguration configuration, IDatabaseService databaseService)
        {
            _logger = logger;
            _configuration = configuration;
            _database = databaseService;
            JobManager.Initialize();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            JobManager.AddJob(
                () => this.ScrapRssFeed(),
                s => s.ToRunNow().AndEvery(12).Hours()
            );
        }

        protected void ScrapRssFeed()
        {
            _logger.LogInformation("Starting scrapper");
            List<Episode> episodes = _database.GetAllEpisodes();
            List<Episode> newEpisodes = new List<Episode>();
            SyndicationFeed feed = null;

            try
            {
                using (var reader = XmlReader.Create(_configuration["RSSFeedURL"]))
                {
                    feed = SyndicationFeed.Load(reader);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            if (feed != null)
            {
                foreach (var element in feed.Items)
                {
                    Episode episode = new Episode();
                    episode.PublishedAt = element.PublishDate.LocalDateTime;
                    foreach (SyndicationElementExtension extension in element.ElementExtensions)
                    {
                        XElement ele = extension.GetObject<XElement>();
                        if (ele.Name.LocalName == "title") episode.Title = ele.Value;
                        if (ele.Name.LocalName == "duration") episode.Duration = TimeToInt(ele.Value);
                        if (ele.Name.LocalName == "explicit") episode.Explicit = ele.Value == "yes" ? true : false;
                        if (ele.Name.LocalName == "episodeId") episode.UUID = ele.Value;
                        if (ele.Name.LocalName == "episodeType") episode.Type = ele.Value;
                        if (ele.Name.LocalName == "episode") episode.EpisodeNumber = Int32.Parse(ele.Value);
                        if (ele.Name.LocalName == "image")
                        {
                            foreach (XAttribute attribute in ele.Attributes())
                            {
                                if (attribute.Name == "href") episode.Image = attribute.Value;
                            }
                        }
                        if (ele.Name.LocalName == "summary") episode.Description = ele.Value;
                    }
                    // Only add episode if it is not in the database yet
                    if (episodes.FindIndex(x => x.UUID == episode.UUID) == -1 && newEpisodes.FindIndex(x => x.UUID == episode.UUID) == -1)
                    {
                        episode.CreatedAt = DateTime.Now;
                        episode.UpdatedAt = DateTime.Now;
                        _logger.LogInformation($"New episode detected (episodeNumber: {episode.Id})");
                        newEpisodes.Add(episode);
                    }
                }
            }
            _logger.LogInformation("Finished scrapper");

            if (newEpisodes.Count != 0)
            {
                _logger.LogInformation($"Inserting {newEpisodes.Count} new values into the database.");
                newEpisodes = newEpisodes.OrderBy(x => x.PublishedAt).ToList();
                try
                {
                    _database.AddEpisodes(newEpisodes);
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    _logger.LogCritical($"Error while inserting new episode into databse. Error:\n{e.Message}");
                }
                _logger.LogInformation($"Inserted {newEpisodes.Count} new values into the database.");
            }
        }

        protected int TimeToInt(string time)
        {
            int result = 0;
            string[] timeStrings = time.Split(':');
            for (int i = 0; i < timeStrings.Length; i++)
            {
                if (timeStrings.Length == 1)
                {
                    result += Int32.Parse(timeStrings[i]);
                }
                if (timeStrings.Length == 2)
                {
                    if (i == 0)
                    {
                        result += Int32.Parse(timeStrings[i]) * 60;
                    }
                    else
                    {
                        result += Int32.Parse(timeStrings[i]);
                    }
                }
                else if (timeStrings.Length == 3)
                {
                    if (i == 0)
                    {
                        result += (Int32.Parse(timeStrings[i]) * 60) * 60;
                    }
                    else if (i == 1)
                    {
                        result += Int32.Parse(timeStrings[i]) * 60;
                    }
                    else
                    {
                        result += Int32.Parse(timeStrings[i]);
                    }
                }
                else if (timeStrings.Length == 4)
                {
                    if (i == 0)
                    {
                        result += ((Int32.Parse(timeStrings[i]) * 60) * 60) * 24;
                    }
                    else if (i == 1)
                    {
                        result += (Int32.Parse(timeStrings[i]) * 60) * 60;
                    }
                    else if (i == 2)
                    {
                        result += Int32.Parse(timeStrings[i]) * 60;
                    }
                    else
                    {
                        result += Int32.Parse(timeStrings[i]);
                    }
                }
                else
                {
                    _logger.LogCritical("TimeToInt does not support weeks");
                    SentrySdk.CaptureMessage("TimeToInt does not support weeks");
                }
            }
            return result;
        }
    }
}
