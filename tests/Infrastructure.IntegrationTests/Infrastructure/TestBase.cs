using CuMusicClub.Application.Services.Telegram;
using CuMusicClub.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CuMusicClub.Infrastructure.IntegrationTests.Infrastructure;

public abstract class TestBase
{
    [SetUp]
    public async Task SetUp()
    {
        await TestApp.ResetState();
    }

    [TearDown]
    public async Task TearDown()
    {
        await CleanUpTelegramTopicsAsync();
    }

    /// <summary>
    /// Удаляет Telegram-топики, созданные тестом, чтобы не засорять тестовую группу.
    /// Топики создаются как побочный эффект полного сбора песни; БД сбрасывается между тестами,
    /// поэтому все <see cref="SongTopic"/> на момент teardown были созданы текущим тестом.
    /// </summary>
    private static async Task CleanUpTelegramTopicsAsync()
    {
        try
        {
            using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
            var telegram = scope.ServiceProvider.GetRequiredService<ITelegramChatService>();
            var topics = scope.ServiceProvider.GetRequiredService<ISongTopicRepository>();

            var created = await topics.Query().ToListAsync();
            foreach (var topic in created)
            {
                try
                {
                    await telegram.DeleteTopic(topic.TopicId);
                }
                catch
                {
                    // Очистка не должна ронять тест.
                }
            }
        }
        catch
        {
            // Очистка не должна ронять тест (например, бот без прав или токена).
        }
    }
}