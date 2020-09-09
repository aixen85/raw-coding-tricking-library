﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrickingLibrary.Data;
using TrickingLibrary.Models;

namespace TrickingLibrary.Api.Controllers
{
    [Route("api/users")]
    [Authorize(TrickingLibraryConstants.Policies.User)]
    public class UserController : ApiController
    {
        private readonly AppDbContext _ctx;

        public UserController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (user != null) return Ok(user);

            user = new User
            {
                Id = userId,
                Username = Username
            };

            _ctx.Add(user);
            await _ctx.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(string id) => Ok();

        [HttpGet("{id}/submissions")]
        public Task<List<Submission>> GetUserSubmissions(string id)
        {
            return _ctx.Submissions
                .Include(x => x.Video)
                .Where(x => x.UserId.Equals(id))
                .ToListAsync();
        }
    }
}