﻿using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FleetCoreMAUI
{
    class ModCreateUserViewModel
    {
        public string _firstname = "";
        public string _surname = "";

        public string firstname
        {
            get { return _firstname; }
            set
            {
                if (_firstname != value)
                {
                    _firstname = value;
                }
            }
        }
        public string surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                }
            }
        }
        public async Task<bool> Create()
        {
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
            var http = devSslHelper.HttpClient;

            try
            {
                string json = await Task.Run(() => JsonConvert.SerializeObject( new CreateUserModel()
                {
                    firstName = firstname,
                    lastName = surname,
                }));
                
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(devSslHelper.DevServerRootUrl + "/api/account/create", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch 
            {
                return false;
            }            
        }
        public ICommand CreateUserCommand =>
        new Command(async () =>
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await Create() is true)
            {
                popup.Close();
                await App.Current.MainPage.DisplayAlert(null, "Użytkownik utworzony pomyślnie", "Ok");
                await Shell.Current.GoToAsync("..");
            } 
            else
            {
                popup.Close();
                Application.Current.MainPage.DisplayAlert(null, "Błąd", "Ok");
            }
        });
    }
}
