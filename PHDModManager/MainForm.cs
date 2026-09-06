using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PHDModManager
{
    public partial class MainForm : Form
    {
        private class CategoryGroup
        {
            public Panel Header;
            public Panel Detail;
            public ExpandButton ExpandBtn;
            public int TargetHeight;
            public Timer AnimTimer;
        }

        private class ModItem
        {
            public string Key;
            public string DisplayName;
            public bool Instalado;
        }

        private List<CategoryGroup> categorias = new List<CategoryGroup>();
        private const int Spacing = 10;

        private string carpetaImages;
        private string carpetaBackup;
        private string carpetaScripts;
        private string carpetaBackupScripts;
        private string carpetaLibreria;

        private const string ArchivoHudCustomizable = "HudPlayerCustomizable.gsc";

        private static readonly string RutaConfig = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PHDModManager", "config.txt"
        );

        // ============================
        // DICCIONARIOS DE NOMBRES
        // ============================
        private static readonly Dictionary<string, string> NombresArmas = new Dictionary<string, string>
        {
            { "dsr50_zm", "DSR-50" },
            { "barretm82_zm", "Barrett M82A1" },
            { "svu_zm", "SVU-AS" },
            { "ballista_zm", "Ballista" },
            { "ak74u_zm", "AK74u" },
            { "mp5k_zm", "MP5K" },
            { "pdw57_zm", "PDW-57" },
            { "qcw05_zm", "Chicom CQB" },
            { "thompson_zm", "Thompson" },
            { "uzi_zm", "Uzi" },
            { "mp40_zm", "MP40" },
            { "mp40_stalker_zm", "MP40 (Stalker)" },
            { "evoskorpion_zm", "Skorpion EVO III" },
            { "fnfal_zm", "FAL" },
            { "m14_zm", "M14" },
            { "saritch_zm", "SMR" },
            { "m16_zm", "M16" },
            { "tar21_zm", "TAR-21" },
            { "gl_tar21_zm", "TAR-21 (Grenade Launcher)" },
            { "galil_zm", "Galil" },
            { "an94_zm", "AN-94" },
            { "type95_zm", "Type 25" },
            { "xm8_zm", "M8A1" },
            { "ak47_zm", "AK-47" },
            { "mp44_zm", "MP-44" },
            { "scar_zm", "SCAR-H" },
            { "hk416_zm", "M27" },
            { "870mcs_zm", "R870 MCS" },
            { "rottweil72_zm", "Olympia" },
            { "saiga12_zm", "S12" },
            { "srm1216_zm", "M1216" },
            { "ksg_zm", "KSG" },
            { "lsat_zm", "LSAT" },
            { "hamr_zm", "HAMR" },
            { "rpd_zm", "RPD" },
            { "minigun_alcatraz_zm", "Death Machine" },
            { "mg08_zm", "MG08" },
            { "m1911_zm", "M1911" },
            { "rnma_zm", "Remington New Model Army" },
            { "judge_zm", "Executioner" },
            { "kard_zm", "Kap-40" },
            { "fiveseven_zm", "Five-Seven" },
            { "fivesevendw_zm", "Five-Seven (Dual Wield)" },
            { "beretta93r_zm", "B23R" },
            { "python_zm", "Python" },
            { "c96_zm", "Mauser C96" },
            { "usrpg_zm", "RPG" },
            { "m32_zm", "War Machine" },
            { "knife_ballistic_zm", "Ballistic Knife" },
            { "knife_ballistic_bowie_zm", "Ballistic Knife (Bowie)" },
            { "knife_ballistic_no_melee_zm", "Ballistic Knife (No Melee)" },
            { "ray_gun_zm", "Ray Gun" },
            { "raygun_mark2_zm", "Ray Gun Mark II" },
            { "slowgun_zm", "Paralyzer" },
            { "slipgun_zm", "Sliquifier" },
            { "blundergat_zm", "Blundergat" },
            { "blundersplat_zm", "Acidgat" },
            { "staff_fire_zm", "Fire Staff" },
            { "staff_water_zm", "Ice Staff" },
            { "staff_air_zm", "Wind Staff" },
            { "staff_lightning_zm", "Lightning Staff" },
            { "staff_revive_zm", "Staff of Revive" },
            { "cymbal_monkey_zm", "Monkey Bomb" },
            { "frag_grenade_zm", "Frag Grenade" },
            { "claymore_zm", "Claymore" },
            { "time_bomb_zm", "Time Bomb" },
            { "sticky_grenade_zm", "Semtex" },
            { "emp_grenade_zm", "EMP Grenade" },
            { "beacon_zm", "Gersh Device" },
            { "bouncing_tomahawk_zm", "Tomahawk" },
            { "knife_zm", "Default Knife" },
            { "bowie_knife_zm", "Bowie Knife" },
            { "tazer_knuckles_zm", "Galvaknuckles" },
            { "spoon_zm_alcatraz", "Spoon" },
            { "spork_zm_alcatraz", "Spork" },
            { "one_inch_punch_zm", "One Inch Punch" },
            { "equip_turbine_zm", "Turbine" },
            { "equip_springpad_zm", "Trample Steam" },
            { "equip_subwoofer_zm", "Sub-Woofer" },
            { "equip_headchopper_zm", "Head Chopper" },
            { "jetgun_zm", "Jet Gun" },
            { "equip_electrictrap_zm", "Electric Trap" },
            { "equip_turret_zm", "Turret" },
            { "equip_dieseldrone_zm", "Maxis Drone" },
            { "riotshield_zm", "Shield (Buried)" },
            { "alcatraz_shield_zm", "Shield (Mob of the Dead)" },
            { "tomb_shield_zm", "Shield (Origins)" },
        };

        private static readonly Dictionary<string, string> NombresPerks = new Dictionary<string, string>
        {
            { "specialty_armorvest", "Juggernog" },
            { "specialty_quickrevive", "Quick Revive" },
            { "specialty_fastreload", "Speed Cola" },
            { "specialty_rof", "Double Tap II" },
            { "specialty_longersprint", "Stamin-Up" },
            { "specialty_additionalprimaryweapon", "Mule Kick" },
            { "specialty_nomotionsensor", "Vulture Aid" },
            { "specialty_finalstand", "Who's Who" },
            { "specialty_grenadepulldeath", "Electric Cherry" },
            { "specialty_ads_zombies", "Deadshot Daiquiri" },
            { "specialty_flakjacket", "PhD Flopper" },
            { "specialty_scavenger", "Tombstone Soda" },
        };

        private static readonly Dictionary<string, string> NombresGuantes = new Dictionary<string, string>
        {
            { "guante_engineer", "Marlton" },
            { "guante_farmergirl", "Misty" },
            { "guante_oldman", "Russman" },
            { "guante_reporter", "Samuel Stuhlinger" },
            { "guante_takeo", "Takeo Masaki" },
            { "guante_richtofen", "Edward Richtofen" },
            { "guante_nikola", "Nikolai Belinski" },
            { "guante_dempsey", "Tank Dempsey" },
            { "guante_deluca", "Sal DeLuca" },
            { "guante_oleary", "Finn O'Leary" },
            { "guante_handsome", "Billy Handsome" },
            { "guante_arlington", "Arlington" },
        };

        // ============================
        // TOKENS PARA DETECTAR ARCHIVOS
        // ============================
        private static readonly Dictionary<string, string> TokenArchivo = new Dictionary<string, string>
        {
            { "dsr50_zm", "sniper_dsr50" },
            { "barretm82_zm", "sniper_m82" },
            { "svu_zm", "sniper_svu" },
            { "ballista_zm", "sniper_ballist" },
            { "ak74u_zm", "smg_ak74u" },
            { "mp5k_zm", "smg_mp5" },
            { "pdw57_zm", "smg_pdw57" },
            { "qcw05_zm", "smg_chicom" },
            { "thompson_zm", "zmb_thompson" },
            { "uzi_zm", "smg_uzi" },
            { "mp40_zm", "smg_mp40" },
            { "evoskorpion_zm", "smg_scorpion" },
            { "fnfal_zm", "ar_fal" },
            { "m14_zm", "ar_m14" },
            { "saritch_zm", "ar_sig556" },
            { "galil_zm", "ar_galil" },
            { "an94_zm", "ar_an94" },
            { "type95_zm", "ar_type95" },
            { "xm8_zm", "ar_xm8" },
            { "ak47_zm", "ar_ak47" },
            { "mp44_zm", "ar_stg44" },
            { "hk416_zm", "ar_hk416" },
            { "870mcs_zm", "shotty_870mcs" },
            { "rottweil72_zm", "shotty_olympia" },
            { "saiga12_zm", "shotty_saiga" },
            { "srm1216_zm", "shotty_srm1216" },
            { "ksg_zm", "shotty_ksg" },
            { "lsat_zm", "lmg_lsat" },
            { "hamr_zm", "lmg_hamr" },
            { "rpd_zm", "lmg_rpd" },
            { "minigun_alcatraz_zm", "minigun" },
            { "mg08_zm", "zmb_mg08" },
            { "m1911_zm", "pistol_m1911" },
            { "rnma_zm", "pistol_rnma" },
            { "judge_zm", "pistol_judge" },
            { "kard_zm", "pistol_kard" },
            { "fiveseven_zm", "pistol_fivesev" },
            { "beretta93r_zm", "pistol_b2023r" },
            { "python_zm", "pistol_python" },
            { "usrpg_zm", "launch_usrpg" },
            { "m32_zm", "launch_m32" },
            { "knife_ballistic_zm", "ballistic_knife" },
            { "ray_gun_zm", "zmb_raygun_col" },
            { "raygun_mark2_zm", "zmb_raygun2" },
            { "slowgun_zm", "zmb_slowgun" },
            { "slipgun_zm", "zmb_slipgun" },
            { "blundergat_zm", "zmb_blundergat_col" },
            { "blundersplat_zm", "zmb_blundergat_acid" },
            { "cymbal_monkey_zm", "monkey_bomb" },
            { "frag_grenade_zm", "grenade_frag" },
            { "time_bomb_zm", "zmb_timebomb" },
            { "sticky_grenade_zm", "grenade_semtex" },
            { "emp_grenade_zm", "grenade_emp" },
            { "beacon_zm", "zmb_beacon" },
            { "bouncing_tomahawk_zm", "zmb_tomahawk" },
            { "knife_zm", "knife_base" },
            { "tazer_knuckles_zm", "taser_knuckles" },
            { "equip_subwoofer_zm", "zmb_subwoofer" },
            { "equip_headchopper_zm", "zmb_chopper" },
            { "jetgun_zm", "zmb_jet_gun" },
            { "tomb_shield_zm", "zmb_shield_col" },
            { "alcatraz_shield_zm", "shield_dlc2" },
            { "riotshield_zm", "shield_dlc4" },
            { "specialty_armorvest", "specialty_juggernaut_zombies" },
            { "specialty_quickrevive", "specialty_quickrevive_zombies" },
            { "specialty_fastreload", "specialty_fastreload_zombies" },
            { "specialty_rof", "specialty_doubletap_zombies" },
            { "specialty_longersprint", "specialty_marathon_zombies" },
            { "specialty_additionalprimaryweapon", "specialty_mulekick_zombies" },
            { "specialty_nomotionsensor", "specialty_vulture_zombies" },
            { "specialty_finalstand", "specialty_whosho_zombies" },
            { "specialty_grenadepulldeath", "specialty_cherry_zombies" },
            { "specialty_scavenger", "specialty_tombstone_zombies" },
            { "specialty_ads_zombies", "specialty_ads_zombies" },
            { "specialty_flakjacket", "specialty_divetonuke_zombies" },
            { "guante_engineer", "zom_engineer" },
            { "guante_farmergirl", "zom_farmergirl" },
            { "guante_oldman", "zom_oldman" },
            { "guante_reporter", "zom_reporter" },
            { "guante_takeo", "zom_takeo" },
            { "guante_richtofen", "zom_richtofen" },
            { "guante_nikola", "zom_nikola" },
            { "guante_dempsey", "zom_dempsey" },
            { "guante_deluca", "zom_deluca" },
            { "guante_oleary", "zom_oleary" },
            { "guante_handsome", "zom_handsome" },
            { "guante_arlington", "zom_arlington" },
        };

        public MainForm()
        {
            InitializeComponent();
            ConfigurarAcordeon();
        }

        private void ConfigurarAcordeon()
        {
            AgregarCategoria(PanelWeapon, DetailWeapons, ExpandButtonWeapons, 120);
            AgregarCategoria(PanelPerks, DetailPerks, ExpandButtonPerks, 140);
            AgregarCategoria(PanelGloves, DetailGlove, ExpandButtonGloves, 120);
            AgregarCategoria(PanelScripts, DetailScripts, ExpandButtonScripts, 90);
            AgregarCategoria(PanelHud, DetailHud, ExpandButtonHud, 90);

            RepositionAll();

            string carpetaBaseT6 = ResolverCarpetaBaseT6();
            if (carpetaBaseT6 == null)
            {
                Application.Exit();
                return;
            }

            carpetaImages = Path.Combine(carpetaBaseT6, "images");
            carpetaScripts = Path.Combine(carpetaBaseT6, "scripts", "zm");

            carpetaBackup = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHDModManager", "DisabledSkins"
            );
            Directory.CreateDirectory(carpetaBackup);

            carpetaBackupScripts = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHDModManager", "DisabledScripts"
            );
            Directory.CreateDirectory(carpetaBackupScripts);

            carpetaLibreria = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHDModManager", "SkinLibrary"
            );
            Directory.CreateDirectory(carpetaLibreria);

            var itemsWeapons = PrepararItems(NombresArmas);
            PoblarListaConVariantes(ItemsWeapons, SearchWeapons, itemsWeapons, "Weapons", ToggleSwitchWeapons);

            var itemsPerks = PrepararItems(NombresPerks);
            PoblarLista(ItemsPerks, SearchPerks, itemsPerks, AplicarEstadoMod, ToggleSwitchPerks);

            var itemsScripts = PrepararItemsScripts();
            PoblarLista(ItemsScripts, SearchScripts, itemsScripts, AplicarEstadoScript, ToggleSwitchScripts);

            var itemsGuantes = PrepararItems(NombresGuantes);
            PoblarLista(ItemsGloves, SearchGloves, itemsGuantes, AplicarEstadoMod, ToggleSwitchGloves);

            var itemsHud = PrepararItemsHud();
            PoblarLista(ItemsHud, SearchHud, itemsHud, AplicarEstadoScript, ToggleSwitchHud);
        }

        private string ResolverCarpetaBaseT6()
        {
            string rutaGuardada = CargarRutaGuardada();
            if (!string.IsNullOrEmpty(rutaGuardada) && Directory.Exists(rutaGuardada))
            {
                return rutaGuardada;
            }

            string rutaEstandar = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Plutonium", "storage", "t6"
            );

            if (Directory.Exists(rutaEstandar))
            {
                GuardarRuta(rutaEstandar);
                return rutaEstandar;
            }

            var resultado = MessageBox.Show(
                "No se encontró la carpeta de Plutonium en la ubicación estándar.\n¿Querés seleccionarla manualmente?",
                "Carpeta no encontrada",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado != DialogResult.Yes)
            {
                MessageBox.Show("Instalá Plutonium primero, o volvé a abrir la app y seleccioná la carpeta.");
                return null;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Seleccioná la carpeta 't6' dentro de Plutonium\\storage (contiene 'images' y 'scripts')";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("No se puede continuar sin la carpeta de Plutonium.");
                    return null;
                }

                string rutaElegida = dialog.SelectedPath;
                GuardarRuta(rutaElegida);
                return rutaElegida;
            }
        }

        private string CargarRutaGuardada()
        {
            try
            {
                if (File.Exists(RutaConfig))
                {
                    string contenido = File.ReadAllText(RutaConfig).Trim();
                    return string.IsNullOrWhiteSpace(contenido) ? null : contenido;
                }
            }
            catch
            {
            }
            return null;
        }

        private void GuardarRuta(string ruta)
        {
            try
            {
                string carpetaConfig = Path.GetDirectoryName(RutaConfig);
                Directory.CreateDirectory(carpetaConfig);
                File.WriteAllText(RutaConfig, ruta);
            }
            catch
            {
            }
        }

        private void AgregarCategoria(Panel header, Panel detail, ExpandButton btn, int targetHeight)
        {
            detail.Height = 0;
            detail.Visible = true;

            var timer = new Timer { Interval = 10 };
            var group = new CategoryGroup { Header = header, Detail = detail, ExpandBtn = btn, TargetHeight = targetHeight, AnimTimer = timer };

            timer.Tick += (s, e) =>
            {
                int target = btn.Expanded ? targetHeight : 0;
                int current = detail.Height;
                int step = Math.Max(1, Math.Abs(target - current) / 4);

                if (current == target)
                {
                    timer.Stop();
                }
                else if (current < target)
                {
                    detail.Height = Math.Min(target, current + step);
                }
                else
                {
                    detail.Height = Math.Max(target, current - step);
                }
                RepositionAll();
            };

            btn.ExpandedChanged += (s, e) => timer.Start();
            categorias.Add(group);
        }

        private void RepositionAll()
        {
            int y = 10;
            foreach (var g in categorias)
            {
                g.Header.Location = new Point(10, y);
                y += g.Header.Height + 4;
                g.Detail.Location = new Point(10, y);
                y += g.Detail.Height;
                y += Spacing;
            }
        }

        // ============================
        // DETECCIÓN DE ARCHIVOS
        // ============================
        private List<string> ObtenerArchivosCoincidentes(string carpeta, string token)
        {
            if (!Directory.Exists(carpeta)) return new List<string>();
            return Directory.GetFiles(carpeta)
                .Where(f => Path.GetFileName(f).ToLower().Contains(token.ToLower()))
                .ToList();
        }

        private List<ModItem> PrepararItems(Dictionary<string, string> nombres)
        {
            var lista = new List<ModItem>();

            foreach (var par in nombres)
            {
                if (!TokenArchivo.TryGetValue(par.Key, out string token))
                    continue;

                bool enImages = ObtenerArchivosCoincidentes(carpetaImages, token).Count > 0;
                bool enBackup = ObtenerArchivosCoincidentes(carpetaBackup, token).Count > 0;

                if (enImages || enBackup)
                {
                    lista.Add(new ModItem { Key = par.Key, DisplayName = par.Value, Instalado = enImages });
                }
            }

            return lista;
        }

        // ============================
        // MOVER ARCHIVOS DE SKINS (activar/desactivar de verdad)
        // ============================
        private void AplicarEstadoMod(string key, bool activar)
        {
            if (!TokenArchivo.TryGetValue(key, out string token))
                return;

            try
            {
                if (activar)
                {
                    var enBackup = ObtenerArchivosCoincidentes(carpetaBackup, token);
                    foreach (var archivo in enBackup)
                    {
                        string destino = Path.Combine(carpetaImages, Path.GetFileName(archivo));
                        File.Move(archivo, destino);
                    }
                }
                else
                {
                    var enImages = ObtenerArchivosCoincidentes(carpetaImages, token);
                    foreach (var archivo in enImages)
                    {
                        string destino = Path.Combine(carpetaBackup, Path.GetFileName(archivo));
                        File.Move(archivo, destino);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo mover el archivo del mod. Cerrá el juego si está abierto e intentá de nuevo.\n\n" + ex.Message,
                    "Error al aplicar el mod",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ============================
        // SISTEMA DE VARIANTES (múltiples skins por ítem)
        // ============================
        private string CarpetaVariantes(string categoria, string key)
        {
            string ruta = Path.Combine(carpetaLibreria, categoria, key);
            Directory.CreateDirectory(ruta);
            return ruta;
        }

        private List<string> ObtenerVariantes(string categoria, string key)
        {
            string carpeta = CarpetaVariantes(categoria, key);
            return Directory.GetDirectories(carpeta).Select(Path.GetFileName).OrderBy(n => n).ToList();
        }

        private string RutaEstadoVariantes => Path.Combine(carpetaLibreria, "estado_variantes.txt");

        private string ObtenerVarianteActivaActual(string categoria, string key)
        {
            if (!File.Exists(RutaEstadoVariantes)) return null;
            string prefijo = $"{categoria}|{key}|";
            foreach (var linea in File.ReadAllLines(RutaEstadoVariantes))
            {
                if (linea.StartsWith(prefijo))
                    return linea.Substring(prefijo.Length);
            }
            return null;
        }

        private void GuardarVarianteActiva(string categoria, string key, string nombreVariante)
        {
            Directory.CreateDirectory(carpetaLibreria);
            string prefijo = $"{categoria}|{key}|";
            var lineas = File.Exists(RutaEstadoVariantes)
                ? File.ReadAllLines(RutaEstadoVariantes).Where(l => !l.StartsWith(prefijo)).ToList()
                : new List<string>();
            lineas.Add(prefijo + nombreVariante);
            File.WriteAllLines(RutaEstadoVariantes, lineas);
        }

        private void RespaldarSkinActual(string categoria, string key, string token)
        {
            var actuales = ObtenerArchivosCoincidentes(carpetaImages, token);
            if (actuales.Count == 0) return;

            string nombreVariante = ObtenerVarianteActivaActual(categoria, key) ?? "Original";
            string carpetaDestino = Path.Combine(CarpetaVariantes(categoria, key), nombreVariante);
            Directory.CreateDirectory(carpetaDestino);

            foreach (var archivo in actuales)
            {
                string destino = Path.Combine(carpetaDestino, Path.GetFileName(archivo));
                if (File.Exists(destino)) File.Delete(destino);
                File.Move(archivo, destino);
            }
        }

        private void AplicarVariante(string categoria, string key, string token, string nombreVariante)
        {
            RespaldarSkinActual(categoria, key, token);

            string carpetaOrigen = Path.Combine(CarpetaVariantes(categoria, key), nombreVariante);
            if (!Directory.Exists(carpetaOrigen)) return;

            foreach (var archivo in Directory.GetFiles(carpetaOrigen))
            {
                string destino = Path.Combine(carpetaImages, Path.GetFileName(archivo));
                File.Move(archivo, destino);
            }

            GuardarVarianteActiva(categoria, key, nombreVariante);
        }

        private void EliminarVariante(string categoria, string key, string nombreVariante)
        {
            if (ObtenerVarianteActivaActual(categoria, key) == nombreVariante)
            {
                MessageBox.Show("No podés eliminar la skin que está puesta actualmente. Cambiá a otra primero.");
                return;
            }

            string carpeta = Path.Combine(CarpetaVariantes(categoria, key), nombreVariante);
            if (Directory.Exists(carpeta))
            {
                Directory.Delete(carpeta, true);
            }
        }

        private void ImportarSkin(string categoria, string key, string token, Action refrescarVista)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = "Seleccioná el archivo de la skin (.iwi)";
                openDialog.Filter = "Archivos IWI (*.iwi)|*.iwi|Todos los archivos (*.*)|*.*";

                if (openDialog.ShowDialog() != DialogResult.OK) return;

                string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nombre para esta skin (ej. Dorada, Neon, Camuflaje):",
                    "Nombre de la skin", "");

                if (string.IsNullOrWhiteSpace(nombre)) return;

                try
                {
                    RespaldarSkinActual(categoria, key, token);

                    string carpetaDestino = Path.Combine(CarpetaVariantes(categoria, key), nombre);
                    Directory.CreateDirectory(carpetaDestino);

                    string nombreArchivo = Path.GetFileName(openDialog.FileName);
                    File.Copy(openDialog.FileName, Path.Combine(carpetaDestino, nombreArchivo), true);

                    AplicarVariante(categoria, key, token, nombre);
                    refrescarVista?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo importar la skin.\n\n" + ex.Message, "Error al importar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ============================
        // SCRIPTS (sin diccionario: se listan directo desde la carpeta)
        // ============================
        private List<ModItem> PrepararItemsScripts()
        {
            var archivos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (Directory.Exists(carpetaScripts))
                foreach (var f in Directory.GetFiles(carpetaScripts))
                    archivos.Add(Path.GetFileName(f));

            if (Directory.Exists(carpetaBackupScripts))
                foreach (var f in Directory.GetFiles(carpetaBackupScripts))
                    archivos.Add(Path.GetFileName(f));

            archivos.RemoveWhere(a => a.Equals(ArchivoHudCustomizable, StringComparison.OrdinalIgnoreCase));

            var lista = new List<ModItem>();
            foreach (var nombreArchivo in archivos)
            {
                bool enScripts = File.Exists(Path.Combine(carpetaScripts, nombreArchivo));
                lista.Add(new ModItem
                {
                    Key = nombreArchivo,
                    DisplayName = Path.GetFileNameWithoutExtension(nombreArchivo),
                    Instalado = enScripts
                });
            }

            return lista.OrderBy(i => i.DisplayName).ToList();
        }

        // ============================
        // HUD PLAYER
        // ============================
        private List<ModItem> PrepararItemsHud()
        {
            var lista = new List<ModItem>();

            bool enScripts = File.Exists(Path.Combine(carpetaScripts, ArchivoHudCustomizable));
            bool enBackup = File.Exists(Path.Combine(carpetaBackupScripts, ArchivoHudCustomizable));

            if (enScripts || enBackup)
            {
                lista.Add(new ModItem
                {
                    Key = ArchivoHudCustomizable,
                    DisplayName = "HUD Player Customizable",
                    Instalado = enScripts
                });
            }

            return lista;
        }

        private void AplicarEstadoScript(string nombreArchivo, bool activar)
        {
            try
            {
                string origen = activar
                    ? Path.Combine(carpetaBackupScripts, nombreArchivo)
                    : Path.Combine(carpetaScripts, nombreArchivo);

                string destino = activar
                    ? Path.Combine(carpetaScripts, nombreArchivo)
                    : Path.Combine(carpetaBackupScripts, nombreArchivo);

                if (File.Exists(origen))
                {
                    File.Move(origen, destino);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo mover el archivo del script. Cerrá el juego si está abierto e intentá de nuevo.\n\n" + ex.Message,
                    "Error al aplicar el script",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ============================
        // LISTA SIMPLE (Perks, Gloves, Scripts, HUD) — sin variantes
        // ============================
        private void PoblarLista(Panel contenedor, TextBox buscador, List<ModItem> items, Action<string, bool> aplicarEstado, ToggleSwitch maestro)
        {
            contenedor.Controls.Clear();
            bool sincronizandoMaestro = false;

            void ActualizarMaestro()
            {
                bool algunoActivo = false;
                foreach (Control fila in contenedor.Controls)
                {
                    foreach (Control child in fila.Controls)
                    {
                        if (child is ToggleSwitch ts && ts.Checked)
                        {
                            algunoActivo = true;
                        }
                    }
                }

                sincronizandoMaestro = true;
                maestro.Checked = algunoActivo;
                sincronizandoMaestro = false;
            }

            int y = 0;
            foreach (var item in items)
            {
                var row = new Panel { Size = new Size(contenedor.Width - 20, 28), Location = new Point(0, y), Tag = item };

                var lbl = new Label { Text = item.DisplayName, AutoSize = true, Location = new Point(4, 6) };
                var toggle = new ToggleSwitch { Size = new Size(40, 20), Location = new Point(row.Width - 50, 4), Checked = item.Instalado };

                toggle.CheckedChanged += (s, e) =>
                {
                    aplicarEstado(item.Key, toggle.Checked);
                    if (!sincronizandoMaestro)
                    {
                        ActualizarMaestro();
                    }
                };

                row.Controls.Add(lbl);
                row.Controls.Add(toggle);
                contenedor.Controls.Add(row);
                y += 32;
            }

            buscador.TextChanged += (s, e) => FiltrarLista(contenedor, buscador.Text);

            maestro.CheckedChanged += (s, e) =>
            {
                if (sincronizandoMaestro) return;

                sincronizandoMaestro = true;
                foreach (Control row in contenedor.Controls)
                {
                    foreach (Control child in row.Controls)
                    {
                        if (child is ToggleSwitch ts && ts.Checked != maestro.Checked)
                        {
                            ts.Checked = maestro.Checked;
                        }
                    }
                }
                sincronizandoMaestro = false;
            };

            ActualizarMaestro();
        }

        // ============================
        // LISTA CON VARIANTES (Weapon Skins por ahora)
        // ============================
        private void PoblarListaConVariantes(Panel contenedor, TextBox buscador, List<ModItem> items, string categoria, ToggleSwitch maestro)
        {
            contenedor.Controls.Clear();
            bool sincronizandoMaestro = false;

            void ActualizarMaestro()
            {
                bool algunoActivo = false;
                foreach (Control fila in contenedor.Controls)
                {
                    foreach (Control child in fila.Controls)
                    {
                        if (child is ToggleSwitch ts && ts.Checked) algunoActivo = true;
                    }
                }
                sincronizandoMaestro = true;
                maestro.Checked = algunoActivo;
                sincronizandoMaestro = false;
            }

            int y = 0;
            foreach (var item in items)
            {
                string token = TokenArchivo.TryGetValue(item.Key, out string t) ? t : null;

                var row = new Panel { Size = new Size(contenedor.Width - 20, 28), Location = new Point(0, y), Tag = item };

                var lbl = new Label { Text = item.DisplayName, AutoSize = true, Location = new Point(4, 6) };

                var btnImportar = new Button { Text = "+", Size = new Size(24, 24), Location = new Point(row.Width - 122, 2), FlatStyle = FlatStyle.Flat };

                var toggle = new ToggleSwitch { Size = new Size(40, 20), Location = new Point(row.Width - 90, 4), Checked = item.Instalado };

                var expandVariantes = new ExpandButton { Size = new Size(24, 24), Location = new Point(row.Width - 30, 2) };

                var panelVariantes = new Panel { Location = new Point(0, 30), Size = new Size(row.Width, 0), AutoScroll = true, Visible = true };

                void RefrescarVariantes()
                {
                    panelVariantes.Controls.Clear();
                    if (token == null) return;

                    int vy = 0;
                    string activa = ObtenerVarianteActivaActual(categoria, item.Key);
                    foreach (var nombreVariante in ObtenerVariantes(categoria, item.Key))
                    {
                        bool esActiva = nombreVariante == activa;
                        var filaVar = new Panel { Size = new Size(row.Width - 10, 24), Location = new Point(4, vy) };

                        var lblVar = new Label
                        {
                            Text = esActiva ? $"{nombreVariante} (en uso)" : nombreVariante,
                            AutoSize = true,
                            Location = new Point(4, 4),
                            ForeColor = esActiva ? Color.FromArgb(46, 125, 50) : Color.Black
                        };
                        filaVar.Controls.Add(lblVar);

                        if (!esActiva)
                        {
                            var btnUsar = new Button { Text = "Usar", Size = new Size(50, 20), Location = new Point(filaVar.Width - 80, 2), FlatStyle = FlatStyle.Flat };
                            btnUsar.Click += (s, e) =>
                            {
                                AplicarVariante(categoria, item.Key, token, nombreVariante);
                                toggle.Checked = true;
                                RefrescarVariantes();
                            };
                            filaVar.Controls.Add(btnUsar);

                            var btnEliminar = new Button { Text = "X", Size = new Size(24, 20), Location = new Point(filaVar.Width - 26, 2), FlatStyle = FlatStyle.Flat };
                            btnEliminar.Click += (s, e) =>
                            {
                                EliminarVariante(categoria, item.Key, nombreVariante);
                                RefrescarVariantes();
                            };
                            filaVar.Controls.Add(btnEliminar);
                        }

                        panelVariantes.Controls.Add(filaVar);
                        vy += 26;
                    }
                    panelVariantes.Height = vy;
                }

                expandVariantes.ExpandedChanged += (s, e) =>
                {
                    if (expandVariantes.Expanded) RefrescarVariantes();
                    panelVariantes.Height = expandVariantes.Expanded ? Math.Max(panelVariantes.Height, 1) : 0;
                    if (!expandVariantes.Expanded) panelVariantes.Controls.Clear();
                };

                btnImportar.Click += (s, e) =>
                {
                    if (token == null) return;
                    ImportarSkin(categoria, item.Key, token, () =>
                    {
                        toggle.Checked = true;
                        if (expandVariantes.Expanded) RefrescarVariantes();
                    });
                };

                toggle.CheckedChanged += (s, e) =>
                {
                    AplicarEstadoMod(item.Key, toggle.Checked);
                    if (!sincronizandoMaestro) ActualizarMaestro();
                };

                row.Controls.Add(lbl);
                row.Controls.Add(btnImportar);
                row.Controls.Add(toggle);
                row.Controls.Add(expandVariantes);
                row.Controls.Add(panelVariantes);
                row.Height = 30;

                contenedor.Controls.Add(row);
                y += 34;
            }

            buscador.TextChanged += (s, e) => FiltrarLista(contenedor, buscador.Text);

            maestro.CheckedChanged += (s, e) =>
            {
                if (sincronizandoMaestro) return;
                sincronizandoMaestro = true;
                foreach (Control row in contenedor.Controls)
                {
                    foreach (Control child in row.Controls)
                    {
                        if (child is ToggleSwitch ts && ts.Checked != maestro.Checked)
                        {
                            ts.Checked = maestro.Checked;
                        }
                    }
                }
                sincronizandoMaestro = false;
            };

            ActualizarMaestro();
        }

        private void FiltrarLista(Panel contenedor, string texto)
        {
            int y = 0;
            foreach (Control row in contenedor.Controls)
            {
                var item = (ModItem)row.Tag;
                bool coincide = item.DisplayName.ToLower().Contains(texto.ToLower());
                row.Visible = coincide;
                if (coincide)
                {
                    row.Location = new Point(0, y);
                    y += 32;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ExpandButtonWeapons_Click(object sender, EventArgs e)
        {

        }

        private void IconSearchWeapons_Click(object sender, EventArgs e)
        {

        }
    }
}