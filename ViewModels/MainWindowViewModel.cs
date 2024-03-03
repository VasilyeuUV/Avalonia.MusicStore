using Avalonia.MusicStore.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Reactive.Concurrency;

namespace Avalonia.MusicStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindowViewModel()
        {
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            // - Инициализация команды
            BuyMusicCommand = ReactiveCommand.Create(async () =>
            {
                var store = new MusicStoreViewModel();
                var result = await ShowDialog.Handle(store);
                if (result != null)
                {
                    Albums.Add(result);
                    await result.SaveToDiskAsync();
                }
            });

            RxApp.MainThreadScheduler.Schedule(LoadAlbums);
        }


        /// <summary>
        /// Список выбранных альбомов
        /// </summary>
        public ObservableCollection<AlbumViewModel> Albums { get; } = new();



        /// <summary>
        /// Команда
        /// </summary>
        public ICommand BuyMusicCommand { get; }

        /// <summary>
        /// Объявление взаимодействия с новым диалоговым окном
        /// </summary>
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }



        private async void LoadAlbums()
        {
            var albums = (await AlbumModel.LoadCachedAsync())
                .Select(x => new AlbumViewModel(x));

            foreach (var album in albums)
            {
                Albums.Add(album);
            }

            foreach (var album in Albums.ToList())
            {
                await album.LoadCover();
            }
        }
    }
}
