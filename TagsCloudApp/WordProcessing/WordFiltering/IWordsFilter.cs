using System.Collections.Generic;

namespace TagsCloudApp.WordFiltering
{
    public interface IWordFilter
    {
        Result<IEnumerable<string>> FilterWords(IEnumerable<string> words);
    }
}
