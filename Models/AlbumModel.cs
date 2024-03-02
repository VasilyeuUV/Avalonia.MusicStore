using iTunesSearch.Library;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Avalonia.MusicStore.Models
{
    public class AlbumModel
    {
        private static iTunesSearchManager s_SearchManager = new();
        private static HttpClient s_httpClient = new();


        public string Artist { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        private string CachePath => $"./Cache/{Artist} - {Title}";


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <param name="coverUrl"></param>
        public AlbumModel(string artist, string title, string coverUrl)
        {
            Artist = artist;
            Title = title;
            CoverUrl = coverUrl;
        }

        public static async Task<IEnumerable<AlbumModel>> SearchAsync(string searchTerm)
        {
            var query = await s_SearchManager
                .GetAlbumsAsync(searchTerm)
                .ConfigureAwait(false);

            return query.Albums.Select(x =>
                new AlbumModel(
                    x.ArtistName, 
                    x.CollectionName,
                    x.ArtworkUrl100.Replace("100x100bb", "600x600bb"))
                );
        }

        /// <summary>
        /// Получение картинок из API
        /// </summary>
        /// <returns> Метод возвращает поток, который можно использовать для загрузки изображения из кэш-файла или API.</returns>
        public async Task<Stream> LoadCoverBitmapAsync()
        {
            if (File.Exists(CachePath + ".bmp"))                            //  - чтение из кэша
            {
                return File.OpenRead(CachePath + ".bmp");                       
            }
            else
            {                                                               //  - чтение из API
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);
            }
        }
    }
}
