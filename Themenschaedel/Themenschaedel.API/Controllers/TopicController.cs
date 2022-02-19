using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;
using User = Themenschaedel.Shared.Models.User;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly IAuthenticationService _auth;
        private readonly ILogger<TopicController> _logger;
        private readonly IClaimService _claims;
        public TopicController(IDatabaseService database, IAuthenticationService auth, ILogger<TopicController> logger, IClaimService claimService)
        {
            _database = database;
            _auth = auth;
            _logger = logger;
            _claims = claimService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<TopicExtended>> GetAllTopics()
        {
            try
            {
                if (!await _auth.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                List<TopicExtended> episodes = await _database.GetAllTopicsAsync();

                if (episodes.Count == 0)
                    return NoContent();

                return Ok(episodes);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError("Error while trying to get all topics. Error:\n" + e.Message);
                return Problem();
            }
        }

        [HttpGet("{episodeId}")]
        public async Task<ActionResult<List<TopicExtended>>> GetTopic(int episodeId)
        {
            try
            {
                if (!await _auth.CheckIfUserHasElivatedPermission(Request)) return Unauthorized();

                List<TopicExtended> topics = await _database.GetTopicsAsync(episodeId);

                if (topics.Count == 0)
                    return NoContent();

                return Ok(topics);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogError("Error while trying to get all topics. Error:\n" + e.Message);
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<TopicExtended>>> PostTopicList([FromBody] TopicRequest request)
        {
            if (request.Topics == null) return BadRequest("Topic list is null!");

            try
            {
                User user = await _auth.GetUserFromValidToken(Request);
                Episode claimedEpisode = await _claims.GetUserByClaimedEpisodeAsync(user.Id);

                // Topics and Subtopics
                await _database.DeleteTopicAndSubtopicAsync(claimedEpisode.Id);
                await _database.ResetIdentityForTopicAndSubtopicsAsync();

                for (int i = 0; i < request.Topics.Count; i++)
                {
                    await _database.InsertTopicAsync(new ProcessedTopicPostRequest(request.Topics[i]), claimedEpisode.Id, user.Id);
                }

                // People
                await _database.DeletePeopleFromEpisodeByEpisodeIdAsync(claimedEpisode.Id);
                await _database.ResetIdentityForPersonInEpisodeTableAsync();
                await _database.InsertPeopleInEpisodeAsync(request.People, claimedEpisode.Id);

                EpisodeExtendedExtra episode = await _database.GetEpisodeAsync(claimedEpisode.Id, true);

                return Ok(episode);
            }
            catch (TokenDoesNotExistException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Sequence contains no elements")) return Unauthorized("User has no claimed episode!");

                SentrySdk.CaptureException(e);
                _logger.LogError("Error while trying to post a list of topics. Error:\n" + e.Message);
                return Problem();
            }
        }

        // ToDo:
        //  Add endpoint to claim an episode under a /claim endpoint -> Make sure a user can only have one claim at a time! (there should be unique constraints on the database anyways lol) -> Default claim time = episode duration * 3 -> DONE!
        //  Add Post TopicExtended (so the client sends the topic and a list of subtopics) -> Make sure the user has claimed the episode he is posting to/editing -> Done!
        //  Add endpoint for clients to extend claim time (for about 30 minutes) -> Maybe a user should only be able to do that 10 times -> 5 more hours in total -> Done
        //  Add way for user/client to remove claim -> already saved stuff will be kept -> Maybe don't add, as users should not claim und unclaim "just to edit another episode", user has to commit to a claimed episode! -> will not be implemented!
        //  Add way for admins to assign other users to abandoned claims -> easily implementable, as the "constraint" of a user only being able to claim a unverified episode with topics is only prohibited by API logic and not by database logic -> see ClaimEpisodeAsync() first 3 lines -> Done
        //  Add way for Admin to remove a claim on an episode -> Done
        //  Add search -> Meilisearch -> Done
        //  Add Hosts endpoint
        //  Reimplement new API on website
    }
}
