﻿using System;
using System.Collections.Generic;
using System.IO;
using DSTEd.Core.Klei;
using DSTEd.UI;
using Indieteur.SAMAPI;
using SteamKit2;

namespace DSTEd.Core.Steam {
    public class Steam {
        private SteamAppsManager software = null;
        private Account account = null;
        private Workshop workshop = null;
        private string path = null;
        private Client client = null;

        public Steam() {
            this.client = new Client();
            this.software = new SteamAppsManager();
            this.account = new Account(this);
            this.workshop = new Workshop();
        }

        public void LoadGame(KleiGame game) {
            Console.WriteLine(string.Format("LoadGame: [#{0}] {1}", game.GetID(), game.GetName()));

            IReadOnlyList<SteamApp> apps = software.SteamApps;
            foreach (SteamApp app in apps) {
                if (game.GetID() == app.AppID) {
                    game.SetPath(app.InstallDir);
                }
            }
        }

        public Boolean ValidatePath(string path) {
            if (path == null) {
                return false;
            }

            return File.Exists(string.Format("{0}{1}Steam.exe", path, Path.DirectorySeparatorChar));
        }

        public Boolean IsInstalled() {
            return this.ValidatePath(this.path);
        }

        public void SetPath(string path) {
            this.path = path;

            if (!this.ValidatePath(this.path) || this.path == null) {
                //this.path = software.InstallDir;
            }
        }

        public Client GetClient() {
            return this.client;
        }

        public String GetPath() {
            return this.path;
        }

        public Account GetAccount() {
            return this.account;
        }

        public Workshop GetWorkShop() {
            return this.workshop;
        }

        public void GetNews(Action<List<KeyValue>> callback) {
            using (dynamic steamNews = WebAPI.GetInterface("ISteamNews")) {
                KeyValue kvNews = steamNews.GetNewsForApp(appid: 322330);
                callback(kvNews["newsitems"]["newsitem"].Children);
            }
        }
    }
}
