using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class PostContentStategyFactory
    {
        private readonly TextContentStrategy _textContentStrategy;
        private readonly ImageContentStrategy _imageContentStrategy;

        public PostContentStategyFactory(
            TextContentStrategy textContentStrategy,
            ImageContentStrategy imageContentStrategy
            )
        {
            _textContentStrategy = textContentStrategy;
            _imageContentStrategy = imageContentStrategy;
        }

        public IPostContentStategy GetByType(ICollection<PostContent> content)
        {
            if (content.Any(x => x.Type == PostType.Image))
            {
                return _imageContentStrategy;
            }
            else
            {
                return _textContentStrategy;
            }
        }
    }
}