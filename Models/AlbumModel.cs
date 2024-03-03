using iTunesSearch.Library;
using iTunesSearch.Library.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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

        #region Код для сохранения на диск

        /// <summary>
        /// Сохраняет текстовые данные альбома как JSON-файл
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            using (var fs = File.OpenWrite(CachePath))
            {
                await SaveToStreamAsync(this, fs);
            }
        }

        /// <summary>
        /// Сохраняет обложку в виде файла изображения формата .BMP.
        /// </summary>
        /// <returns></returns>
        public Stream SaveCoverBitmapStream()
        {
            return File.OpenWrite(CachePath + ".bmp");
        }


        private static async Task SaveToStreamAsync(AlbumModel data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
        }

        #endregion // Код для сохранения на диск



        //#################################################################################################
        #region Код реализации загрузки с диска

        public static async Task<Album> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false))!;
        }

        public static async Task<IEnumerable<Album>> LoadCachedAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            var results = new List<Album>();

            foreach (var file in Directory.EnumerateFiles("./Cache"))
            {
                if (!string.IsNullOrWhiteSpace(new DirectoryInfo(file).Extension)) continue;

                await using var fs = File.OpenRead(file);
                results.Add(await AlbumModel.LoadFromStream(fs).ConfigureAwait(false));
            }

            return results;
        }


        #endregion // Код реализации загрузки с диска
    }
}
