using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

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
                }
            });
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
    }
}
