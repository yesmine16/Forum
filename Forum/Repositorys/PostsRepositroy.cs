using Forum.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Repositorys
{
    public class PostsRepositroy : DbContext, IRepository<Post>
    {
        private readonly IConfiguration _configuration;

        public PostsRepositroy(DbContextOptions<PostsRepositroy> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Post> Posts { get; set; }

        public async Task<List<Post>> GetAll()
        {
            if (Posts == null)
            {
                throw new InvalidOperationException();
            }

            return await Posts.ToListAsync();
        }

        public async Task<List<PostDetailsAggregation>> GetAllWithDetails()
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"];

            var detailsList = new List<PostDetailsAggregation>();
            await using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sqlQuery = @"
                SELECT 
                    Posts.Id AS PostId,
                    Posts.Message,
                    Posts.DateCreationMessage,
                    Themes.id AS ThemeId,
                    Themes.titre AS ThemeTitle,
                    Users.Id AS UserId,
                    Users.Pseudonyme,
                    Users.Email,
                    Users.CheminAvatar,
                    Posts.Topic
                FROM 
                    Posts
                JOIN 
                    Themes ON posts.themeid = Themes.id
                JOIN 
                    Users ON users.id = posts.userid;";

            await using (var command = new SqlCommand(sqlQuery, connection))
            {
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var postId = reader.GetGuid(reader.GetOrdinal("PostId"));
                        var message = reader.GetString(reader.GetOrdinal("Message"));
                        var dateCreationMessage = reader.GetDateTime(reader.GetOrdinal("DateCreationMessage"));
                        var themeId = reader.GetGuid(reader.GetOrdinal("ThemeId"));
                        var themeTitle = reader.GetString(reader.GetOrdinal("ThemeTitle"));
                        var userId = reader.GetGuid(reader.GetOrdinal("UserId"));
                        var pseudonyme = reader.GetString(reader.GetOrdinal("Pseudonyme"));
                        var email = reader.GetString(reader.GetOrdinal("Email"));
                        var topic = reader.GetString(reader.GetOrdinal("Topic"));
                        var cheminAvatar = reader.GetString(reader.GetOrdinal("CheminAvatar"));
                        var detail = new PostDetailsAggregation
                        {
                            Id = postId,
                            DateCreationMessage = dateCreationMessage,
                            Message = message,
                            Theme = themeTitle,
                            UserName = pseudonyme,
                            ThemeId = themeId,
                            UserId = userId,
                            Topic = topic,
                            PostId = postId,
                            CheminAvatar = cheminAvatar,
                        };

                        detailsList.Add(detail);

                    }
                }
            }

            return detailsList;
        }

        public async Task<List<PostDetailsAggregation>> GetAllWithDetails(Guid id)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"];

            var detailsList = new List<PostDetailsAggregation>();
            await using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sqlQuery = @"
                SELECT 
                    Posts.Id AS PostId,
                    Posts.Message,
                    Posts.DateCreationMessage,
                    Themes.id AS ThemeId,
                    Themes.titre AS ThemeTitle,
                    Users.Id AS UserId,
                    Users.Pseudonyme,
                    Users.Email,
                    Posts.Topic
                FROM 
                    Posts
                JOIN 
                    Themes ON posts.themeid = Themes.id
                JOIN 
                    Users ON users.id = posts.userid
                 WHERE
                      Users.Id = @UserId";

            await using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@UserId", id);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var postId = reader.GetGuid(reader.GetOrdinal("PostId"));
                        var message = reader.GetString(reader.GetOrdinal("Message"));
                        var dateCreationMessage = reader.GetDateTime(reader.GetOrdinal("DateCreationMessage"));
                        var themeId = reader.GetGuid(reader.GetOrdinal("ThemeId"));
                        var themeTitle = reader.GetString(reader.GetOrdinal("ThemeTitle"));
                        var userId = reader.GetGuid(reader.GetOrdinal("UserId"));
                        var pseudonyme = reader.GetString(reader.GetOrdinal("Pseudonyme"));
                        var email = reader.GetString(reader.GetOrdinal("Email"));
                        var topic = reader.GetString(reader.GetOrdinal("Topic"));
                        var detail = new PostDetailsAggregation
                        {
                            Id = postId,
                            DateCreationMessage = dateCreationMessage,
                            Message = message,
                            Theme = themeTitle,
                            UserName = pseudonyme,
                            ThemeId = themeId,
                            UserId = userId,
                            Topic = topic
                        };

                        detailsList.Add(detail);

                    }
                }
            }

            return detailsList;
        }


        public async Task<PostDetailsAggregation> GetDetailsById(Guid id)
        {
            if (Posts == null)
            {
                throw new InvalidOperationException();
            }

            var list =  await GetAllWithDetails();
            
            return list.FirstOrDefault(postDetail => postDetail.Id == id);
        }


        public async Task<List<PostDetailsAggregation>> GetPostsByUserName(string userName)
        {
            var posts = await GetAllWithDetails();
            var userPosts = posts.Where(post => post.UserName == userName).OrderBy(post => post.DateCreationMessage).ToList();
            return userPosts;
        }
        public async Task<Post> GetById(Guid id)
        {
            if (Posts == null)
            {
                throw new InvalidOperationException();
            }

            return await Posts.FirstOrDefaultAsync(post => post.Id == id) ?? throw new ArgumentNullException();
        }

        public async Task<Post> Update(Post newObject)
        {
            var existingPost = await GetById(newObject.Id);

            ArgumentNullException.ThrowIfNull(existingPost);

            existingPost.Topic = newObject.Topic;
            existingPost.Message = newObject.Message;

            await SaveChangesAsync();
            return existingPost;
        }

        public async Task DeleteById(Guid id)
        {
            var post = await Posts.FirstOrDefaultAsync(u => u.Id == id);
            ArgumentNullException.ThrowIfNull(post);
            Posts.Remove(post);
            await SaveChangesAsync();
        }

        public async Task CreatePost(Post post)
        {
            var counter = 0;
            retry:

            try
            {
                Posts.Add(post);
                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (counter <= 3)
                {
                    counter++;
                    goto retry;
                }
            }
        }


        public async Task<List<PostDetailsAggregation>> GetPostByTheme(Guid themeId)
        {
            var details = await GetAllWithDetails();
            var posts = details
                .Where(p => p.ThemeId == themeId)
                .OrderBy(post => post.DateCreationMessage)
                .ToList();

            return posts;

        }

    }
}
