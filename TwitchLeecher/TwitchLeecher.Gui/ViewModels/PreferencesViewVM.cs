﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;
using TwitchLeecher.Services.Interfaces;
using TwitchLeecher.Shared.Commands;

namespace TwitchLeecher.Gui.ViewModels
{
    public class PreferencesViewVM : ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly IPreferencesService _preferencesService;

        private Preferences _currentPreferences;

        private ICommand _addFavouriteChannelCommand;
        private ICommand _removeFavouriteChannelCommand;
        private ICommand _chooseDownloadTempFolderCommand;
        private ICommand _chooseDownloadFolderCommand;
        private ICommand _chooseExternalPlayerCommand;
        private ICommand _clearExternalPlayerCommand;
        private ICommand _saveCommand;
        private ICommand _undoCommand;
        private ICommand _defaultsCommand;

        private readonly object _commandLockObject;

        #endregion Fields

        #region Constructors

        public PreferencesViewVM(
            IDialogService dialogService,
            INotificationService notificationService,
            IPreferencesService preferencesService)
        {
            _dialogService = dialogService;
            _notificationService = notificationService;
            _preferencesService = preferencesService;

            _commandLockObject = new object();
        }

        #endregion Constructors

        #region Properties

        public Preferences CurrentPreferences
        {
            get
            {
                if (_currentPreferences == null)
                {
                    _currentPreferences = _preferencesService.CurrentPreferences.Clone();
                }

                return _currentPreferences;
            }

            private set
            {
                SetProperty(ref _currentPreferences, value);
            }
        }

        public ICommand AddFavouriteChannelCommand
        {
            get
            {
                if (_addFavouriteChannelCommand == null)
                {
                    _addFavouriteChannelCommand = new DelegateCommand(AddFavouriteChannel);
                }

                return _addFavouriteChannelCommand;
            }
        }

        public ICommand RemoveFavouriteChannelCommand
        {
            get
            {
                if (_removeFavouriteChannelCommand == null)
                {
                    _removeFavouriteChannelCommand = new DelegateCommand(RemoveFavouriteChannel);
                }

                return _removeFavouriteChannelCommand;
            }
        }

        public ICommand ChooseDownloadTempFolderCommand
        {
            get
            {
                if (_chooseDownloadTempFolderCommand == null)
                {
                    _chooseDownloadTempFolderCommand = new DelegateCommand(ChooseDownloadTempFolder);
                }

                return _chooseDownloadTempFolderCommand;
            }
        }

        public ICommand ChooseDownloadFolderCommand
        {
            get
            {
                if (_chooseDownloadFolderCommand == null)
                {
                    _chooseDownloadFolderCommand = new DelegateCommand(ChooseDownloadFolder);
                }

                return _chooseDownloadFolderCommand;
            }
        }

        public ICommand ChooseExternalPlayerCommand
        {
            get
            {
                if (_chooseExternalPlayerCommand == null)
                {
                    _chooseExternalPlayerCommand = new DelegateCommand(ChooseExternalPlayer);
                }

                return _chooseExternalPlayerCommand;
            }
        }

        public ICommand ClearExternalPlayerCommand
        {
            get
            {
                if (_clearExternalPlayerCommand == null)
                {
                    _clearExternalPlayerCommand = new DelegateCommand(ClearExternalPlayer);
                }

                return _clearExternalPlayerCommand;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand(Save);
                }

                return _saveCommand;
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                if (_undoCommand == null)
                {
                    _undoCommand = new DelegateCommand(Undo);
                }

                return _undoCommand;
            }
        }

        public ICommand DefaultsCommand
        {
            get
            {
                if (_defaultsCommand == null)
                {
                    _defaultsCommand = new DelegateCommand(Defaults);
                }

                return _defaultsCommand;
            }
        }

        #endregion Properties

        #region Methods

        private void AddFavouriteChannel()
        {
            try
            {
                lock (_commandLockObject)
                {
                    string currentChannel = CurrentPreferences.SearchChannelName;

                    if (!string.IsNullOrWhiteSpace(currentChannel))
                    {
                        string existingEntry = CurrentPreferences.SearchFavouriteChannels.FirstOrDefault(channel => channel.Equals(currentChannel, StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrWhiteSpace(existingEntry))
                        {
                            CurrentPreferences.SearchChannelName = existingEntry;
                        }
                        else
                        {
                            CurrentPreferences.SearchFavouriteChannels.Add(currentChannel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void RemoveFavouriteChannel()
        {
            try
            {
                lock (_commandLockObject)
                {
                    string currentChannel = CurrentPreferences.SearchChannelName;

                    if (!string.IsNullOrWhiteSpace(currentChannel))
                    {
                        string existingEntry = CurrentPreferences.SearchFavouriteChannels.FirstOrDefault(channel => channel.Equals(currentChannel, StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrWhiteSpace(existingEntry))
                        {
                            CurrentPreferences.SearchFavouriteChannels.Remove(existingEntry);
                            CurrentPreferences.SearchChannelName = CurrentPreferences.SearchFavouriteChannels.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseDownloadTempFolder()
        {
            try
            {
                lock (_commandLockObject)
                {
                    _dialogService.ShowFolderBrowserDialog(CurrentPreferences.DownloadTempFolder, ChooseDownloadTempFolderCallback);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseDownloadTempFolderCallback(bool cancelled, string folder)
        {
            try
            {
                if (!cancelled)
                {
                    CurrentPreferences.DownloadTempFolder = folder;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseDownloadFolder()
        {
            try
            {
                lock (_commandLockObject)
                {
                    _dialogService.ShowFolderBrowserDialog(CurrentPreferences.DownloadFolder, ChooseDownloadFolderCallback);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseDownloadFolderCallback(bool cancelled, string folder)
        {
            try
            {
                if (!cancelled)
                {
                    CurrentPreferences.DownloadFolder = folder;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseExternalPlayer()
        {
            try
            {
                lock (_commandLockObject)
                {
                    var filter = new CommonFileDialogFilter("Executables", "*.exe");
                    _dialogService.ShowFileBrowserDialog(filter, CurrentPreferences.MiscExternalPlayer, ChooseExternalPlayerCallback);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ClearExternalPlayer()
        {
            try
            {
                lock (_commandLockObject)
                {
                    CurrentPreferences.MiscExternalPlayer = null;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ChooseExternalPlayerCallback(bool cancelled, string file)
        {
            try
            {
                if (!cancelled)
                {
                    CurrentPreferences.MiscExternalPlayer = file;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void Save()
        {
            try
            {
                lock (_commandLockObject)
                {
                    _dialogService.SetBusy();
                    Validate();

                    if (!HasErrors)
                    {
                        _preferencesService.Save(_currentPreferences);
                        CurrentPreferences = null;
                        _notificationService.ShowNotification("기본 설정이 저장되었습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void Undo()
        {
            try
            {
                lock (_commandLockObject)
                {
                    MessageBoxResult result = _dialogService.ShowMessageBox("현재 변경 내용을 취소하고 마지막으로 저장된 기본 설정을 다시 불러오시겠습니까?", "되돌리기", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dialogService.SetBusy();
                        CurrentPreferences = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void Defaults()
        {
            try
            {
                lock (_commandLockObject)
                {
                    MessageBoxResult result = _dialogService.ShowMessageBox("기본 설정을 불러오시겠습니까?", "기본 설정", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dialogService.SetBusy();
                        _preferencesService.Save(_preferencesService.CreateDefault());
                        CurrentPreferences = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(CurrentPreferences);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                CurrentPreferences?.Validate();

                if (CurrentPreferences.HasErrors)
                {
                    AddError(currentProperty, "잘못된 기본 설정입니다!");
                }
            }
        }

        public override void OnBeforeHidden()
        {
            try
            {
                CurrentPreferences = null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        protected override List<MenuCommand> BuildMenu()
        {
            List<MenuCommand> menuCommands = base.BuildMenu();

            if (menuCommands == null)
            {
                menuCommands = new List<MenuCommand>();
            }

            menuCommands.Add(new MenuCommand(SaveCommand, "저장", "Save"));
            menuCommands.Add(new MenuCommand(UndoCommand, "되돌리기", "Undo"));
            menuCommands.Add(new MenuCommand(DefaultsCommand, "기본 값", "Wrench"));

            return menuCommands;
        }

        #endregion Methods
    }
}