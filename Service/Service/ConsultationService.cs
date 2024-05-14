using Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API.Completions;
using OpenAI_API;
using Core.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Core.Data.DTO;
using Microsoft.Extensions.Configuration;
namespace Service.Service
{
    public class ConsultationService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration _configuration;
        public ConsultationService()
        {
        }

        public ConsultationService(ApplicationDbContext context, IConfiguration config)
        {
            this.context = context;
            _configuration = config;
        }
        public async Task<string> GetConsultation(string response)
        {
            string outputResult = "";
            var openai = new OpenAIAPI(_configuration.GetSection("OpenAPI:Key").Value);
            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = response;
            completionRequest.Model = OpenAI_API.Models.Model.ChatGPTTurboInstruct;
            completionRequest.MaxTokens = 1024;

            var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

            foreach (var completion in completions.Completions)
            {
                outputResult += completion.Text;
            }

            return outputResult;

        }

        public async Task<List<ConsultationQuestionModel>> GetQuestion()
        {
            var questions = await context.ConsultationQuestions.ToListAsync();
            return questions;
        }
    }
}
