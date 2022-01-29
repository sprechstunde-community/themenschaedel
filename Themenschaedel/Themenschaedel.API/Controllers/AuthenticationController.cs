﻿using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models.Request;

namespace Themenschaedel.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDatabaseService _databaseService;

        public AuthenticationController(IAuthenticationService authenticationService, IDatabaseService databaseService)
        {
            _authenticationService = authenticationService;
            _databaseService = databaseService;
        }

        /*
        // POST api/<EpisodesController>
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody] UserLogin user)
        {
            if (user.Username.Length > 30) return BadRequest("Username is exceeding username lenght limit!");
            if (user.Username.Length == 0 || user.Username == "") return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Username)) return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Password)) return BadRequest("Invalid Password!");

            
            bool wasUserRegistrationSuccessful = await _authenticationService.Login(user);

            if (wasUserRegistrationSuccessful)
            {
                return Ok();
            }
            else
            {
                return Problem("An error occured, please try again or report the error.");
            }
            
    }
        */


        // POST api/<EpisodesController>
        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody] UserRegistration user)
        {
            if (user.Email == "") return BadRequest("Email is empty!");
            if (!IsValidEmailAddress(user.Email)) return BadRequest("The Email address is invalid!");
            if (user.Username.Length > 30) return BadRequest("Username is exceeding username lenght limit!");
            if (user.Username.Length == 0 || user.Username == "") return BadRequest("Username cannot be empty!");
            if (String.IsNullOrWhiteSpace(user.Username)) return BadRequest("Username cannot be empty!");
            if (user.Password.Length <= 8) return BadRequest("A password has to be more than 8 characters!");
            if (user.Password.Length > 127) return BadRequest("A password cannot be more than 127 characters!");
            if (!await _databaseService.IsRegistrationEmailUnique(user.Email)) return BadRequest("A user with this email already exists!");
            if (!await _databaseService.IsRegistrationUsernameUnique(user.Username)) return BadRequest("A user with this username already exists!");

            bool wasUserRegistrationSuccessful = await _authenticationService.Register(user);

            if (wasUserRegistrationSuccessful)
            {
                return Ok();
            }
            else
            {
                return Problem("An error occured, please try again or report the error.");
            }
        }

        protected bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
