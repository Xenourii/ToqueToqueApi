using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Helpers;
using ToqueToqueApi.Models;
using ToqueToqueApi.Services;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;

        public ConversationsController(IConversationService conversationService, IMapper mapper)
        {
            _conversationService = conversationService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var conversationsDb = _conversationService.GetAll();
            var conversations = _mapper.Map<IList<Conversation>>(conversationsDb);
            return Ok(conversations);
        }

        [HttpGet("me")]
        public IActionResult GetMyConversations()
        {
            var userId = GetUserIdFromAuthentication();
            var conversations = _conversationService.GetAllFromUser(userId);
            return Ok(conversations);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var userId = GetUserIdFromAuthentication();
                var conversationsDb = _conversationService.GetById(id, userId);
                var conversations = _mapper.Map<Conversation>(conversationsDb);
                return Ok(conversations);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ConversationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Conversation conversationToCreate)
        {
            var conversation = _mapper.Map<ConversationDb>(conversationToCreate);

            try
            {
                var createdDbConversation = _conversationService.Create(conversation);
                var createdConversation = _mapper.Map<Conversation>(createdDbConversation);
                return CreatedAtAction(nameof(Create), createdConversation);
            }
            catch (ConversationException e)
            {
                return BadRequest(new {message = e.Message});
            }
        }

        [HttpDelete]
        public IActionResult Remove(int id)
        {
            try
            {
                _conversationService.Delete(id);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        [HttpGet("{id}/messages")]
        public IActionResult GetMessagesFromConversation(int id)
        {
            var messages = _conversationService.GetMessagesFromConversation(id);
            return Ok(messages);
        }

        protected int GetUserIdFromAuthentication()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                throw new NotAuthorizedException("No identity token found");

            return JWTHelper.GetUserId(identity);
        }
    }
}