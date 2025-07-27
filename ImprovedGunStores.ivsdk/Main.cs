using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using IVSDKDotNet;
using IVSDKDotNet.Attributes;
using static IVSDKDotNet.Native.Natives;
using CCL;
using IVSDKDotNet.Enums;
using System.Threading;
using System.Runtime;
using System.Numerics;
using CCL.GTAIV;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using static System.Net.WebRequestMethods;
using ImprovedGunStores.ivsdk;
using System.Diagnostics;
using IVSDKDotNet.Native;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Xml.Linq;
using System.Security.Policy;

namespace ImprovedGunStores
{
    public class Main : Script
    {
        // Consts
        private int numOfAttachmts = 3;

        // PlayerShit
        public static IVPed PlayerPed { get; set; }
        public static int PlayerIndex { get; set; }
        public static int PlayerHandle { get; set; }
        public static Vector3 PlayerPos { get; set; }

        // IniShit
        private static bool enable;
        private static bool showBuyMarkers;
        private static bool episodicCheck;
        private static int numOfWeaponIDs;
        private static string weaponModel;
        private static string weaponName;
        private static Vector3 weapPos;
        private static Vector3 weapRot;
        private static Vector3 weapBuyPos;
        private static float weapBuyRad;
        private static float weapBuyHdg;
        private static string buySound;
        private static string dealerMdl;
        private static int dealerWpn;
        private static Vector3 dealerPos;
        private static float dealerHdg;
        private static Vector3 guardPos;
        private static int cooldown;
        private static int gSpawnDelay;
        private static bool isWeapUnlocked;
        private static string roomName;
        private static string gRoomName;
        private static int doorHash;
        private static Vector3 doorPos;
        private static int hudR;
        private static int hudG;
        private static int hudB;
        private static int hudTextR;
        private static int hudTextG;
        private static int hudTextB;
        private static int weapCanSpawn;
        private static int clipAmmo;
        private static Vector3 backroomPos = Vector3.Zero;
        private static float backroomHdg = 0;
        private static bool backroomStock;
        private static string weapMsg;
        private static bool unlockDYHP;
        private static bool buyAmmoEarly;
        private static bool keepAmmo;

        // Attachment
        private static int attachmentPrice;
        //private static int grenLaunPrice;
        private static int grenPrice;
        //private static int scopePrice;

        // Lists,Arrays
        private static string[] locConfFiles;
        private static int[] WeaponProps;
        private static Vector3[] weapCoords;
        private static int[] GuardPeds;
        private static uint[] attackTimers;
        private static bool[] weapUnlocks;

        // Attachment arrays
        private static bool[][] attachmentUnlocks;
        private static bool[] canBuyAttachment;
        private static bool[] ownsAttachment;
        private static bool[] hasAttachment;

        private static List<Vector3> locations = new List<Vector3>();
        private static List<SettingsFile> gunStores = new List<SettingsFile>();
        private static List<bool> storeHostile = new List<bool>();
        private static List<string> guardMdl = new List<string>();
        private static List<int> guardWpn = new List<int>();
        private static List<int> backroomWeaps = new List<int>();

        // BooleanTriggerStuffIDK
        private static bool settingExists;
        private static bool HasSpawned = false;
        private static bool canInspect = false;
        private static bool isInspecting = false;
        private static bool bFailMsg = false;
        private static bool camActivate;
        private static bool menuActive;
        private static bool goInspect;
        private static bool confirmation = false;
        private static bool enterGunShop = false;
        private static bool bInspectMsg = false;
        private static bool bComparing = false;
        private static bool bSpawnDealer = false;
        private static bool bSpawnGuards = false;
        private static bool bIsHostile = false;
        private static bool hasConfigsLoaded = false;
        private static bool extraWeap = false;
        private static bool spawnWeaponInAir = false;
        private static bool canBuyStock = false;

        // AttachmentBools
        //private static bool hasScopeAttachment = false;
        //private static bool hasGLAttachment = false;
        //private static bool hasThisAttachment = false;
        private static bool attachmentMenu = false;
        //private static bool ownsGLAttachment = false;

        // GameStuff
        private static uint currEp;
        private static SettingsFile configFile;
        private static SettingsFile attachmentsConfig;
        private static SettingsFile weapFuncsConfig;
        private static string locPath;
        private static SettingsFile statConfFile;
        private static SettingsFile msgConfFile;
        private static Vector3 location;
        private static int MissionProgress;
        private static int camera = 0;
        private static int backroomCount = 0;
        private static int backroomSelectedWeap = 0;
        private static int weapInAir = 0;
        private static int grenAmmo = 0;
        private static int currAttachmt = 0;

        private static uint gTimer;
        private static uint fTimer;
        private static uint rKey;
        private static uint pKey;

        private static Vector3 wCoords;
        private static float wRad;
        private static float animTime;

        private static int weapID = 0;
        private static int pAmmo = 0;
        private static int maxAmmo = 0;

        private static uint pMoney;
        private static int animStart = 0;
        private static int animLoop = 0;
        private static int animEnd = 0;

        private static int weapSlot;
        private static int weapInSlot;
        private static int ammoInSlot;
        private static int maxAmmoInSlot;
        private static int soundID = -1;
        private static int weapChecked = 0;

        // WeaponStatShit

        private static string pWeaponName;

        private static int weapCost = 0;
        private static int ammoCost = 0;

        private static string statTextA = "";
        private static string statTextB = "";
        private static string statTextC = "";
        private static string statTextD = "";
        private static string statTextE = "";
        private static string statTextF = "";
        private static string statTextG = "";
        private static string statTextH = "";

        private static int weapStatA = -1;
        private static int weapStatB = -1;
        private static int weapStatC = -1;
        private static int weapStatD = -1;
        private static int weapStatE = -1;
        private static int weapStatF = -1;
        private static int weapStatG = -1;
        private static int weapStatH = -1;

        private static int weapBarA = -1;
        private static int weapBarB = -1;
        private static int weapBarC = -1;
        private static int weapBarD = -1;
        private static int weapBarE = -1;
        private static int weapBarF = -1;
        private static int weapBarG = -1;
        private static int weapBarH = -1;

        private static string pStatTextA = "";
        private static string pStatTextB = "";
        private static string pStatTextC = "";
        private static string pStatTextD = "";
        private static string pStatTextE = "";
        private static string pStatTextF = "";
        private static string pStatTextG = "";
        private static string pStatTextH = "";

        private static int pWeapStatA = -1;
        private static int pWeapStatB = -1;
        private static int pWeapStatC = -1;
        private static int pWeapStatD = -1;
        private static int pWeapStatE = -1;
        private static int pWeapStatF = -1;
        private static int pWeapStatG = -1;
        private static int pWeapStatH = -1;

        private static int pWeapBarA = -1;
        private static int pWeapBarB = -1;
        private static int pWeapBarC = -1;
        private static int pWeapBarD = -1;
        private static int pWeapBarE = -1;
        private static int pWeapBarF = -1;
        private static int pWeapBarG = -1;
        private static int pWeapBarH = -1;

        private static int weapPellets = 0;
        private static int pWeapPellets = 0;

        // Mission Shit
        float GameProg = GET_FLOAT_STAT(0);

        float RomanProg = GET_FLOAT_STAT(3);
        float VladProg = GET_FLOAT_STAT(4);
        float JacobProg = GET_FLOAT_STAT(7);
        float FaustProg = GET_FLOAT_STAT(8);
        float MannyProg = GET_FLOAT_STAT(9);
        float ElizProg = GET_FLOAT_STAT(10);
        float DwayneProg = GET_FLOAT_STAT(13);
        float BrucieProg = GET_FLOAT_STAT(16);
        float PBoyXProg = GET_FLOAT_STAT(17);
        float FrncsProg = GET_FLOAT_STAT(18);
        float ULPProg = GET_FLOAT_STAT(19);
        float PackieProg = GET_FLOAT_STAT(22);
        float RayBoProg = GET_FLOAT_STAT(23);
        float GerryProg = GET_FLOAT_STAT(24);
        float DerrickProg = GET_FLOAT_STAT(25);
        float BernieProg = GET_FLOAT_STAT(26);
        float BellProg = GET_FLOAT_STAT(27);
        float GambtiProg = GET_FLOAT_STAT(28);
        float JimmyProg = GET_FLOAT_STAT(29);

        float BillyProg = GET_FLOAT_STAT(121);
        float JimProg = GET_FLOAT_STAT(122);
        float StubbsProg = GET_FLOAT_STAT(125);
        float AshleyProg = GET_FLOAT_STAT(126);
        float LizTProg = GET_FLOAT_STAT(127);
        float RayBProg = GET_FLOAT_STAT(128);

        float TonyProg = GET_FLOAT_STAT(188);
        float YusufProg = GET_FLOAT_STAT(191);
        float MoriProg = GET_FLOAT_STAT(192);
        float ArmandoProg = GET_FLOAT_STAT(198);
        float BulgarinProg = GET_FLOAT_STAT(200);
        float RoccoProg = GET_FLOAT_STAT(201);

        int mostWantedProg = GET_INT_STAT(360);
        int pigeonsKilled = GET_INT_STAT(361);
        int jacDrugProg = GET_INT_STAT(284);
        int stuntJumpProg = GET_INT_STAT(270);
        int stevieProg = GET_INT_STAT(285);
        int impExpProg = GET_INT_STAT(469);

        int seagullsKilledTLAD = GET_INT_STAT(143);
        int stubbsSideProg = GET_INT_STAT(145);
        int gangWarTLAD = GET_INT_STAT(177);

        int seagullsKilledTBoGT = GET_INT_STAT(211);
        int nightClubProg = GET_INT_STAT(244);
        int gangWarTBoGT = GET_INT_STAT(243);

        public Main()
        {
            Uninitialize += Main_Uninitialize;
            Initialized += Main_Initialized;
            GameLoad += Main_GameLoad;
            Tick += Main_Tick;
        }
        private void Main_GameLoad(object sender, EventArgs e)
        {
            for (int i = 0; i < locations.Count(); i++)
                storeHostile[i] = false;

            LoadSettings(Settings);

            GameProg = GET_FLOAT_STAT(0);

            RomanProg = GET_FLOAT_STAT(3);
            VladProg = GET_FLOAT_STAT(4);
            JacobProg = GET_FLOAT_STAT(7);
            FaustProg = GET_FLOAT_STAT(8);
            MannyProg = GET_FLOAT_STAT(9);
            ElizProg = GET_FLOAT_STAT(10);
            DwayneProg = GET_FLOAT_STAT(13);
            BrucieProg = GET_FLOAT_STAT(16);
            PBoyXProg = GET_FLOAT_STAT(17);
            FrncsProg = GET_FLOAT_STAT(18);
            ULPProg = GET_FLOAT_STAT(19);
            PackieProg = GET_FLOAT_STAT(22);
            RayBoProg = GET_FLOAT_STAT(23);
            GerryProg = GET_FLOAT_STAT(24);
            DerrickProg = GET_FLOAT_STAT(25);
            BernieProg = GET_FLOAT_STAT(26);
            BellProg = GET_FLOAT_STAT(27);
            GambtiProg = GET_FLOAT_STAT(28);
            JimmyProg = GET_FLOAT_STAT(29);

            BillyProg = GET_FLOAT_STAT(121);
            JimProg = GET_FLOAT_STAT(122);
            StubbsProg = GET_FLOAT_STAT(125);
            AshleyProg = GET_FLOAT_STAT(126);
            LizTProg = GET_FLOAT_STAT(127);
            RayBProg = GET_FLOAT_STAT(128);

            TonyProg = GET_FLOAT_STAT(188);
            YusufProg = GET_FLOAT_STAT(191);
            MoriProg = GET_FLOAT_STAT(192);
            ArmandoProg = GET_FLOAT_STAT(198);
            BulgarinProg = GET_FLOAT_STAT(200);
            RoccoProg = GET_FLOAT_STAT(201);

            mostWantedProg = GET_INT_STAT(360);
            pigeonsKilled = GET_INT_STAT(361);
            jacDrugProg = GET_INT_STAT(284);
            stuntJumpProg = GET_INT_STAT(270);
            stevieProg = GET_INT_STAT(285);
            impExpProg = GET_INT_STAT(469);

            seagullsKilledTLAD = GET_INT_STAT(143);
            stubbsSideProg = GET_INT_STAT(145);
            gangWarTLAD = GET_INT_STAT(177);

            seagullsKilledTBoGT = GET_INT_STAT(211);
            nightClubProg = GET_INT_STAT(244);
            gangWarTBoGT = GET_INT_STAT(243);
        }
        private void GetGameProgress()
        {
            if (currEp == 0)
                GameProg = GET_FLOAT_STAT(0);
            else if (currEp == 1)
                GameProg = GET_FLOAT_STAT(133);
            else if (currEp == 2)
                GameProg = GET_FLOAT_STAT(187);
        }
        private void LoadColors(SettingsFile settings)
        {
            if (currEp == 0) {
                hudR = settings.GetInteger("MAIN", "IVStatNameR", 0);
                hudG = settings.GetInteger("MAIN", "IVStatNameG", 0);
                hudB = settings.GetInteger("MAIN", "IVStatNameB", 0);
                hudTextR = settings.GetInteger("MAIN", "IVStatTextR", 0);
                hudTextG = settings.GetInteger("MAIN", "IVStatTextG", 0);
                hudTextB = settings.GetInteger("MAIN", "IVStatTextB", 0);
            }
            else if (currEp == 1)
            {
                hudR = settings.GetInteger("MAIN", "TLADStatNameR", 0);
                hudG = settings.GetInteger("MAIN", "TLADStatNameG", 0);
                hudB = settings.GetInteger("MAIN", "TLADStatNameB", 0);
                hudTextR = settings.GetInteger("MAIN", "TLADStatTextR", 0);
                hudTextG = settings.GetInteger("MAIN", "TLADStatTextG", 0);
                hudTextB = settings.GetInteger("MAIN", "TLADStatTextB", 0);
            }
            else if (currEp == 2)
            {
                hudR = settings.GetInteger("MAIN", "TBOGTStatNameR", 0);
                hudG = settings.GetInteger("MAIN", "TBOGTStatNameG", 0);
                hudB = settings.GetInteger("MAIN", "TBOGTStatNameB", 0);
                hudTextR = settings.GetInteger("MAIN", "TBOGTStatTextR", 0);
                hudTextG = settings.GetInteger("MAIN", "TBOGTStatTextG", 0);
                hudTextB = settings.GetInteger("MAIN", "TBOGTStatTextB", 0);
            }
        }
        public static string GetAttachmentName(int attachID)
        {
            switch (attachID)
            {
                case 0:
                    return "GrenadeLauncher";
                case 1:
                    return "Scope";
                default:
                    return "";
            }
        }
        private void WriteBooleanToINI(SettingsFile settings, string name, bool booleShit)
        {
            if (!settings.DoesSectionExists(IVGenericGameStorage.ValidSaveName))
                settings.AddSection(IVGenericGameStorage.ValidSaveName);
            if (!settings.DoesKeyExists(IVGenericGameStorage.ValidSaveName, name))
                settings.AddKeyToSection(IVGenericGameStorage.ValidSaveName, name);
            settings.SetBoolean(IVGenericGameStorage.ValidSaveName, name, booleShit);
        }
        private void WriteIntToINI(SettingsFile settings, string name, int integerShit)
        {
            if (!settings.DoesSectionExists(IVGenericGameStorage.ValidSaveName))
                settings.AddSection(IVGenericGameStorage.ValidSaveName);
            if (!settings.DoesKeyExists(IVGenericGameStorage.ValidSaveName, name))
                settings.AddKeyToSection(IVGenericGameStorage.ValidSaveName, name);
            settings.SetInteger(IVGenericGameStorage.ValidSaveName, name, integerShit);
        }
        private void Main_Uninitialize(object sender, EventArgs e)
        {
            gunStores.Clear();
            locations.Clear();
            storeHostile.Clear();
            guardMdl.Clear();
            guardWpn.Clear();
            backroomWeaps.Clear();
            DespawnShit();
        }
        private void Main_Initialized(object sender, EventArgs e)
        {
            gunStores.Clear();
            locations.Clear();
            storeHostile.Clear();
            guardMdl.Clear();
            guardWpn.Clear();
            backroomWeaps.Clear();

            locPath = string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\LocationData", IVGame.GameStartupPath);
            locConfFiles = System.IO.Directory.GetFiles(locPath);
            statConfFile = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\WeaponStats.txt", IVGame.GameStartupPath));
            statConfFile.Load();
            msgConfFile = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\WeaponMessages.txt", IVGame.GameStartupPath));
            msgConfFile.Load();
            attachmentsConfig = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\Attachments.ini", IVGame.GameStartupPath));
            attachmentsConfig.Load();
            if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                weapFuncsConfig = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath));
            else
                weapFuncsConfig = attachmentsConfig;

            weapFuncsConfig.Load();

            foreach (string fileName in locConfFiles)
                gunStores.Add(new SettingsFile(fileName));

            for (int i = 0; i < gunStores.Count(); i++)
            {
                gunStores[i].Load();
                Vector3 loc = gunStores[i].GetVector3("MAIN", "Location", Vector3.Zero);
                locations.Add(loc);
                storeHostile.Add(false);
            }

            LoadSettings(Settings);
            WeaponProps = new int[numOfWeaponIDs];
            GuardPeds = new int[4];
            weapCoords = new Vector3[numOfWeaponIDs];
            attackTimers = new uint[locations.Count()];
        }
        private int CreateObject_DontRequestModel(int hash, float x, float y, float z, float heading)
        {
            int obj;
            CREATE_OBJECT_NO_OFFSET(hash, x, y, z, out obj, true);
            SET_OBJECT_HEADING(obj, heading);
            return obj;
        }
        private int CreatePed_DontRequestModel(int modelHash, Vector3 pos, float heading, int pedType)
        {
            int ped;

            CREATE_CHAR(pedType, modelHash, pos.X, pos.Y, pos.Z, out ped, true);
            SET_CHAR_HEADING(ped, heading);

            return ped;
        }
        private void CreateCamera(int cam, bool active, float x, float y, float z, Vector3 pointAt, float fov)
        {
            float pOffX, pOffY, pOffZ;

            CREATE_CAM(14, out cam);
            GET_OFFSET_FROM_CHAR_IN_WORLD_COORDS(Main.PlayerHandle, x, y, z, out pOffX, out pOffY, out pOffZ);
            SET_CAM_POS(cam, pOffX, pOffY, pOffZ);
            POINT_CAM_AT_COORD(cam, pointAt.X, pointAt.Y, pointAt.Z);
            SET_CAM_FOV(cam, fov);
            SET_CAM_ACTIVE(cam, active);
            SET_CAM_PROPAGATE(cam, active);
            ACTIVATE_SCRIPTED_CAMS(active, active);
            DISPLAY_HUD(!active);
            DISPLAY_RADAR(!active);
        }
        private void RequestAnims()
        {
            if (!HAVE_ANIMS_LOADED("missgunlockup"))
                REQUEST_ANIMS("missgunlockup");
            if (!HAVE_ANIMS_LOADED("pickup_object"))
                REQUEST_ANIMS("pickup_object");
        }
        private void LoadSettings(SettingsFile settings)
        {
            enable = settings.GetBoolean("MAIN", "Enable", false);
            showBuyMarkers = settings.GetBoolean("MAIN", "ShowMarkers", false);
            numOfWeaponIDs = settings.GetInteger("MAIN", "WeaponIDAmt", 60);
            episodicCheck = settings.GetBoolean("MAIN", "CheckEpisodicWeapons", false);
            cooldown = settings.GetInteger("MAIN", "HostileCooldown", 0);
            gSpawnDelay = settings.GetInteger("MAIN", "GuardSpawnDelay", 0);
            weapUnlocks = new bool[numOfWeaponIDs];
            attachmentUnlocks = new bool[numOfWeaponIDs][];

            for (int i = 0; i < numOfWeaponIDs; i++)
            {
                attachmentUnlocks[i] = new bool[numOfAttachmts];
            }
            hasAttachment = new bool[numOfAttachmts];
            ownsAttachment = new bool[numOfAttachmts];

            canBuyAttachment = new bool[numOfAttachmts + 1];
            unlockDYHP = settings.GetBoolean("MAIN", "UnlockAfterDYHP", false);
            buyAmmoEarly = settings.GetBoolean("MAIN", "BuyAmmoBeforeUnlock", false);
            keepAmmo = settings.GetBoolean("MAIN", "KeepAmmo", false);

            for (int i = 0; i < numOfWeaponIDs; i++)
            {
                if (settings.DoesSectionExists(i.ToString()))
                {
                    if (!settings.DoesSectionExists(IVGenericGameStorage.ValidSaveName))
                        settings.AddSection(IVGenericGameStorage.ValidSaveName);
                    if (!settings.DoesKeyExists(IVGenericGameStorage.ValidSaveName, i.ToString() + "Unlocked"))
                    {
                        settings.AddKeyToSection(IVGenericGameStorage.ValidSaveName, i.ToString() + "Unlocked");
                        settings.SetBoolean(IVGenericGameStorage.ValidSaveName, i.ToString() + "Unlocked", false);
                    }

                    weapUnlocks[i] = settings.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "Unlocked"), false);

                    attachmentUnlocks[i][0] = settings.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "HasGrenadeLauncherAttachment"), false);
                    attachmentUnlocks[i][1] = settings.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "HasScopeAttachment"), false);
                }
            }
        }
        private void LoadConfigs(int config)
        {
            configFile = gunStores[config];
            dealerPos = configFile.GetVector3("MAIN", "DealerPosition", Vector3.Zero);
            dealerHdg = configFile.GetFloat("MAIN", "DealerHeading", 0);
            dealerWpn = configFile.GetInteger("MAIN", "DealerWeapon", 0);
            guardPos = configFile.GetVector3("MAIN", "GuardSpawnLoc", Vector3.Zero);
            dealerMdl = configFile.GetValue("MAIN", "DealerModel", "");
            roomName = configFile.GetValue("MAIN", "RoomName", "");
            gRoomName = configFile.GetValue("MAIN", "GuardRoomName", "");
            doorHash = configFile.GetInteger("MAIN", "DoorHash", 0);
            doorPos = configFile.GetVector3("MAIN", "DoorLocation", Vector3.Zero);
            backroomPos = configFile.GetVector3("MAIN", "BackroomPos", Vector3.Zero);
            backroomHdg = configFile.GetFloat("MAIN", "BackroomHdg", 0);
        }
        private void LoadWeaponPosition(SettingsFile settings, int weapon)
        {
            if (settings.DoesSectionExists(weapon.ToString()))
            {
                if (currEp == 0)
                {
                    weaponModel = settings.GetValue(weapon.ToString(), "IVWeaponModel", "");
                    weapCanSpawn = settings.GetInteger(weapon.ToString(), "IVUnlockAfter", 0);
                }
                else if (currEp == 1)
                {
                    weaponModel = settings.GetValue(weapon.ToString(), "TLADWeaponModel", "");
                    weapCanSpawn = settings.GetInteger(weapon.ToString(), "TLADUnlockAfter", 0);
                }
                else if (currEp == 2)
                {
                    weaponModel = settings.GetValue(weapon.ToString(), "TBOGTWeaponModel", "");
                    weapCanSpawn = settings.GetInteger(weapon.ToString(), "TBOGTUnlockAfter", 0);
                }
                weapPos = configFile.GetVector3(weapon.ToString(), "WeaponPosition", Vector3.Zero);
                weapRot = configFile.GetVector3(weapon.ToString(), "WeaponRotation", Vector3.Zero);
                backroomStock = settings.GetBoolean(weapon.ToString(), "BackroomStock", false);
                if (weapCanSpawn >= 0)
                    SpawnTheWeapon(weapon);
                if (backroomStock)
                    backroomWeaps.Add(weapon);
            }
        }
        private void LoadWeaponBuyPos(SettingsFile settings, int weapon)
        {
            weapBuyPos = configFile.GetVector3(weapon.ToString(), "BuyPosition", Vector3.Zero);
            weapBuyRad = configFile.GetFloat(weapon.ToString(), "BuyRadius", 0);
            weapBuyHdg = configFile.GetFloat(weapon.ToString(), "BuyHeading", 0);
        }
        private void GetWeaponData(SettingsFile settings, int weapon)
        {
            int i;
            for (i = 0; i < numOfWeaponIDs; i++)
            {
                if (settings.DoesSectionExists(weapon.ToString()))
                {
                    weapUnlocks[i] = settings.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "Unlocked"), false);
                    weaponName = settings.GetValue(weapon.ToString(), "Name", "");
                    clipAmmo = settings.GetInteger(weapon.ToString(), "ClipAmmo", 0);
                    //isWeapUnlocked = settings.GetBoolean(IVGenericGameStorage.ValidSaveName, (weapon.ToString() + "Unlocked"), false);
                    isWeapUnlocked = weapUnlocks[weapon];
                }

                animStart = configFile.GetInteger(weapon.ToString(), "AnimStart", 0);
                animLoop = configFile.GetInteger(weapon.ToString(), "AnimLoop", 0);
                animEnd = configFile.GetInteger(weapon.ToString(), "AnimEnd", 0);
                weapCost = configFile.GetInteger(weapon.ToString(), "WeaponPrice", 0);
                ammoCost = configFile.GetInteger(weapon.ToString(), "AmmoPrice", 0);
                buySound = settings.GetValue(weapon.ToString(), "WeaponBuySound", "");
                weapMsg = msgConfFile.GetValue(weapon.ToString(), "Message", "");

                statTextA = statConfFile.GetValue(weapon.ToString(), "StatTextA", "");
                statTextB = statConfFile.GetValue(weapon.ToString(), "StatTextB", "");
                statTextC = statConfFile.GetValue(weapon.ToString(), "StatTextC", "");
                statTextD = statConfFile.GetValue(weapon.ToString(), "StatTextD", "");
                statTextE = statConfFile.GetValue(weapon.ToString(), "StatTextE", "");
                statTextF = statConfFile.GetValue(weapon.ToString(), "StatTextF", "");
                statTextG = statConfFile.GetValue(weapon.ToString(), "StatTextG", "");
                statTextH = statConfFile.GetValue(weapon.ToString(), "StatTextH", "");

                weapStatA = statConfFile.GetInteger(weapon.ToString(), "StatA", -1);
                weapStatB = statConfFile.GetInteger(weapon.ToString(), "StatB", -1);
                weapStatC = statConfFile.GetInteger(weapon.ToString(), "StatC", -1);
                weapStatD = statConfFile.GetInteger(weapon.ToString(), "StatD", -1);
                weapStatE = statConfFile.GetInteger(weapon.ToString(), "StatE", -1);
                weapStatF = statConfFile.GetInteger(weapon.ToString(), "StatF", -1);
                weapStatG = statConfFile.GetInteger(weapon.ToString(), "StatG", -1);
                weapStatH = statConfFile.GetInteger(weapon.ToString(), "StatH", -1);

                weapBarA = statConfFile.GetInteger(weapon.ToString(), "BarA", -1);
                weapBarB = statConfFile.GetInteger(weapon.ToString(), "BarB", -1);
                weapBarC = statConfFile.GetInteger(weapon.ToString(), "BarC", -1);
                weapBarD = statConfFile.GetInteger(weapon.ToString(), "BarD", -1);
                weapBarE = statConfFile.GetInteger(weapon.ToString(), "BarE", -1);
                weapBarF = statConfFile.GetInteger(weapon.ToString(), "BarF", -1);
                weapBarG = statConfFile.GetInteger(weapon.ToString(), "BarG", -1);
                weapBarH = statConfFile.GetInteger(weapon.ToString(), "BarH", -1);

                //Attachments
                for (int a = 0; a < numOfAttachmts; a++) {
                    hasAttachment[a] = attachmentsConfig.GetBoolean(weapon.ToString(), "CanBuy" + GetAttachmentName(a) + "Attachment", false);
                    if (hasAttachment[a])
                        canBuyAttachment[a] = true;
                    else
                        canBuyAttachment[a] = false;
                }
                /*hasGLAttachment = attachmentsConfig.GetBoolean(weapon.ToString(), "CanBuyGLAttachment", false);
                if (hasGLAttachment)
                    canBuyAttachment[0] = true;
                else
                    canBuyAttachment[0] = false;

                hasScopeAttachment = attachmentsConfig.GetBoolean(weapon.ToString(), "CanBuyScopeAttachment", false);
                if (hasScopeAttachment)
                    canBuyAttachment[1] = true;
                else
                    canBuyAttachment[1] = false;*/

                ownsAttachment[currAttachmt] = attachmentUnlocks[weapon][currAttachmt];
                //ownsAttachment[1] = attachmentUnlocks[weapon][1];

                attachmentPrice = attachmentsConfig.GetInteger(weapon.ToString(), GetAttachmentName(currAttachmt) + "Price", 0);
                grenPrice = attachmentsConfig.GetInteger(weapon.ToString(), "GrenadePrice", 0);
            }
        }
        private void GetCurrWeaponData(SettingsFile settings, int weapon)
        {
            int i;
            for (i = 0; i < numOfWeaponIDs; i++)
            {
                pWeaponName = settings.GetValue(weapon.ToString(), "Name", "");

                pStatTextA = statConfFile.GetValue(weapon.ToString(), "StatTextA", "");
                pStatTextB = statConfFile.GetValue(weapon.ToString(), "StatTextB", "");
                pStatTextC = statConfFile.GetValue(weapon.ToString(), "StatTextC", "");
                pStatTextD = statConfFile.GetValue(weapon.ToString(), "StatTextD", "");
                pStatTextE = statConfFile.GetValue(weapon.ToString(), "StatTextE", "");
                pStatTextF = statConfFile.GetValue(weapon.ToString(), "StatTextF", "");
                pStatTextG = statConfFile.GetValue(weapon.ToString(), "StatTextG", "");
                pStatTextH = statConfFile.GetValue(weapon.ToString(), "StatTextH", "");

                pWeapStatA = statConfFile.GetInteger(weapon.ToString(), "StatA", -1);
                pWeapStatB = statConfFile.GetInteger(weapon.ToString(), "StatB", -1);
                pWeapStatC = statConfFile.GetInteger(weapon.ToString(), "StatC", -1);
                pWeapStatD = statConfFile.GetInteger(weapon.ToString(), "StatD", -1);
                pWeapStatE = statConfFile.GetInteger(weapon.ToString(), "StatE", -1);
                pWeapStatF = statConfFile.GetInteger(weapon.ToString(), "StatF", -1);
                pWeapStatG = statConfFile.GetInteger(weapon.ToString(), "StatG", -1);
                pWeapStatH = statConfFile.GetInteger(weapon.ToString(), "StatH", -1);

                pWeapBarA = statConfFile.GetInteger(weapon.ToString(), "BarA", -1);
                pWeapBarB = statConfFile.GetInteger(weapon.ToString(), "BarB", -1);
                pWeapBarC = statConfFile.GetInteger(weapon.ToString(), "BarC", -1);
                pWeapBarD = statConfFile.GetInteger(weapon.ToString(), "BarD", -1);
                pWeapBarE = statConfFile.GetInteger(weapon.ToString(), "BarE", -1);
                pWeapBarF = statConfFile.GetInteger(weapon.ToString(), "BarF", -1);
                pWeapBarG = statConfFile.GetInteger(weapon.ToString(), "BarG", -1);
                pWeapBarH = statConfFile.GetInteger(weapon.ToString(), "BarH", -1);
            }
        }
        private void CheckWeaponUnlock(SettingsFile settings)
        {
            for (int i = 0; i < numOfWeaponIDs; i++)
            {
                if (!settings.DoesSectionExists(i.ToString()))
                    continue;

                int weapUnlock = 0;
                if (currEp == 0)
                    weapUnlock = settings.GetInteger(i.ToString(), "IVUnlockAfter", 0);

                else if (currEp == 1)
                    weapUnlock = settings.GetInteger(i.ToString(), "TLADUnlockAfter", 0);

                else if (currEp == 2)
                    weapUnlock = settings.GetInteger(i.ToString(), "TBOGTUnlockAfter", 0);

                if (weapUnlock == MissionProgress || weapUnlock == 0)
                    weapUnlocks[i] = true;
                    //WriteBooleanToINI(Settings, i.ToString(), true);
            }
        }
        private void CheckSideMissions()
        {
            if (currEp == 0)
            {
                int mwKill = GET_INT_STAT(360);
                int ratKill = GET_INT_STAT(361);
                int drugDeliver = GET_INT_STAT(284);
                int stuntJump = GET_INT_STAT(270);
                int stevieCar = GET_INT_STAT(285);
                int brucieCar = GET_INT_STAT(469);

                if (mwKill != mostWantedProg)
                {
                    if (mwKill >= 30)
                        MissionProgress = 91;
                    else if (mwKill >= 25)
                        MissionProgress = 90;
                    else if (mwKill >= 20)
                        MissionProgress = 89;
                    else if (mwKill >= 15)
                        MissionProgress = 88;
                    else if (mwKill >= 10)
                        MissionProgress = 87;
                    else if (mwKill >= 5)
                        MissionProgress = 86;
                    mostWantedProg = mwKill;
                    CheckWeaponUnlock(Settings);
                }
                else if (ratKill != pigeonsKilled)
                {
                    if (ratKill >= 200)
                        MissionProgress = 131;
                    else if (ratKill >= 195)
                        MissionProgress = 130;
                    else if (ratKill >= 190)
                        MissionProgress = 129;
                    else if (ratKill >= 185)
                        MissionProgress = 128;
                    else if (ratKill >= 180)
                        MissionProgress = 127;
                    else if (ratKill >= 175)
                        MissionProgress = 126;
                    else if (ratKill >= 170)
                        MissionProgress = 125;
                    else if (ratKill >= 165)
                        MissionProgress = 124;
                    else if (ratKill >= 160)
                        MissionProgress = 123;
                    else if (ratKill >= 155)
                        MissionProgress = 122;
                    else if (ratKill >= 150)
                        MissionProgress = 121;
                    else if (ratKill >= 145)
                        MissionProgress = 120;
                    else if (ratKill >= 140)
                        MissionProgress = 119;
                    else if (ratKill >= 135)
                        MissionProgress = 118;
                    else if (ratKill >= 130)
                        MissionProgress = 117;
                    else if (ratKill >= 125)
                        MissionProgress = 116;
                    else if (ratKill >= 120)
                        MissionProgress = 115;
                    else if (ratKill >= 115)
                        MissionProgress = 114;
                    else if (ratKill >= 110)
                        MissionProgress = 113;
                    else if (ratKill >= 105)
                        MissionProgress = 112;
                    else if (ratKill >= 100)
                        MissionProgress = 111;
                    else if (ratKill >= 95)
                        MissionProgress = 110;
                    else if (ratKill >= 90)
                        MissionProgress = 109;
                    else if (ratKill >= 85)
                        MissionProgress = 108;
                    else if (ratKill >= 80)
                        MissionProgress = 107;
                    else if (ratKill >= 75)
                        MissionProgress = 106;
                    else if (ratKill >= 70)
                        MissionProgress = 105;
                    else if (ratKill >= 65)
                        MissionProgress = 104;
                    else if (ratKill >= 60)
                        MissionProgress = 103;
                    else if (ratKill >= 55)
                        MissionProgress = 102;
                    else if (ratKill >= 50)
                        MissionProgress = 101;
                    else if (ratKill >= 45)
                        MissionProgress = 100;
                    else if (ratKill >= 40)
                        MissionProgress = 99;
                    else if (ratKill >= 35)
                        MissionProgress = 98;
                    else if (ratKill >= 30)
                        MissionProgress = 97;
                    else if (ratKill >= 25)
                        MissionProgress = 96;
                    else if (ratKill >= 20)
                        MissionProgress = 95;
                    else if (ratKill >= 15)
                        MissionProgress = 94;
                    else if (ratKill >= 10)
                        MissionProgress = 93;
                    else if (ratKill >= 5)
                        MissionProgress = 92;
                    pigeonsKilled = ratKill;
                    CheckWeaponUnlock(Settings);
                }
                else if (drugDeliver != jacDrugProg)
                {
                    if (drugDeliver >= 10)
                        MissionProgress = 141;
                    else if (drugDeliver >= 9)
                        MissionProgress = 140;
                    else if (drugDeliver >= 8)
                        MissionProgress = 139;
                    else if (drugDeliver >= 7)
                        MissionProgress = 138;
                    else if (drugDeliver >= 6)
                        MissionProgress = 137;
                    else if (drugDeliver >= 5)
                        MissionProgress = 136;
                    else if (drugDeliver >= 4)
                        MissionProgress = 135;
                    else if (drugDeliver >= 3)
                        MissionProgress = 134;
                    else if (drugDeliver >= 2)
                        MissionProgress = 133;
                    else if (drugDeliver >= 1)
                        MissionProgress = 132;
                    jacDrugProg = drugDeliver;
                    CheckWeaponUnlock(Settings);
                }
                else if (stuntJump != stuntJumpProg)
                {
                    if (stuntJump >= 50)
                        MissionProgress = 151;
                    else if (stuntJump >= 45)
                        MissionProgress = 150;
                    else if (stuntJump >= 40)
                        MissionProgress = 149;
                    else if (stuntJump >= 35)
                        MissionProgress = 148;
                    else if (stuntJump >= 30)
                        MissionProgress = 147;
                    else if (stuntJump >= 25)
                        MissionProgress = 146;
                    else if (stuntJump >= 20)
                        MissionProgress = 145;
                    else if (stuntJump >= 15)
                        MissionProgress = 144;
                    else if (stuntJump >= 10)
                        MissionProgress = 143;
                    else if (stuntJump >= 5)
                        MissionProgress = 142;
                    stuntJumpProg = stuntJump;
                    CheckWeaponUnlock(Settings);
                }
                else if (stevieCar != stevieProg)
                {
                    if (stevieCar >= 30)
                        MissionProgress = 157;
                    else if (stevieCar >= 25)
                        MissionProgress = 156;
                    else if (stevieCar >= 20)
                        MissionProgress = 155;
                    else if (stevieCar >= 15)
                        MissionProgress = 154;
                    else if (stevieCar >= 10)
                        MissionProgress = 153;
                    else if (stevieCar >= 5)
                        MissionProgress = 152;
                    stevieProg = stevieCar;
                    CheckWeaponUnlock(Settings);
                }
                else if (brucieCar != impExpProg)
                {
                    if (brucieCar >= 10)
                        MissionProgress = 167;
                    else if (brucieCar >= 9)
                        MissionProgress = 166;
                    else if (brucieCar >= 8)
                        MissionProgress = 165;
                    else if (brucieCar >= 7)
                        MissionProgress = 164;
                    else if (brucieCar >= 6)
                        MissionProgress = 163;
                    else if (brucieCar >= 5)
                        MissionProgress = 162;
                    else if (brucieCar >= 4)
                        MissionProgress = 161;
                    else if (brucieCar >= 3)
                        MissionProgress = 160;
                    else if (brucieCar >= 2)
                        MissionProgress = 159;
                    else if (brucieCar >= 1)
                        MissionProgress = 158;
                    impExpProg = brucieCar;
                    CheckWeaponUnlock(Settings);
                }
            }
            else if (currEp == 1)
            {
                int seagull = GET_INT_STAT(143);
                int stubbsShit = GET_INT_STAT(145);
                int gangWar = GET_INT_STAT(177);

                if (seagull != seagullsKilledTLAD)
                {
                    if (seagull >= 50)
                        MissionProgress = 32;
                    else if (seagull >= 45)
                        MissionProgress = 31;
                    else if (seagull >= 40)
                        MissionProgress = 30;
                    else if (seagull >= 35)
                        MissionProgress = 29;
                    else if (seagull >= 30)
                        MissionProgress = 28;
                    else if (seagull >= 25)
                        MissionProgress = 27;
                    else if (seagull >= 20)
                        MissionProgress = 26;
                    else if (seagull >= 15)
                        MissionProgress = 25;
                    else if (seagull >= 10)
                        MissionProgress = 24;
                    else if (seagull >= 5)
                        MissionProgress = 23;
                    seagullsKilledTLAD = seagull;
                    CheckWeaponUnlock(Settings);
                }
                else if (stubbsShit != stubbsSideProg)
                {
                    if (stubbsShit >= 5)
                        MissionProgress = 37;
                    else if (stubbsShit >= 4)
                        MissionProgress = 36;
                    else if (stubbsShit >= 3)
                        MissionProgress = 35;
                    else if (stubbsShit >= 2)
                        MissionProgress = 34;
                    else if (stubbsShit >= 1)
                        MissionProgress = 33;
                    stubbsSideProg = stubbsShit;
                    CheckWeaponUnlock(Settings);
                }
                else if (gangWar != gangWarTLAD)
                {
                    if (gangWar >= 25)
                        MissionProgress = 42;
                    else if (gangWar >= 20)
                        MissionProgress = 41;
                    else if (gangWar >= 15)
                        MissionProgress = 40;
                    else if (gangWar >= 10)
                        MissionProgress = 39;
                    else if (gangWar >= 5)
                        MissionProgress = 38;
                    gangWarTLAD = gangWar;
                    CheckWeaponUnlock(Settings);
                }
            }
            else if (currEp == 2)
            {
                int seagull = GET_INT_STAT(211);
                int nightClub = GET_INT_STAT(244);
                int gangWar = GET_INT_STAT(243);
                
                if (seagull != seagullsKilledTBoGT)
                {
                    if (seagull >= 50)
                        MissionProgress = 34;
                    else if (seagull >= 45)
                        MissionProgress = 33;
                    else if (seagull >= 40)
                        MissionProgress = 32;
                    else if (seagull >= 35)
                        MissionProgress = 31;
                    else if (seagull >= 30)
                        MissionProgress = 30;
                    else if (seagull >= 25)
                        MissionProgress = 29;
                    else if (seagull >= 20)
                        MissionProgress = 28;
                    else if (seagull >= 15)
                        MissionProgress = 27;
                    else if (seagull >= 10)
                        MissionProgress = 26;
                    else if (seagull >= 5)
                        MissionProgress = 25;
                    seagullsKilledTBoGT = seagull;
                }
                else if (nightClub != nightClubProg)
                {
                    if (nightClub >= 8)
                        MissionProgress = 42;
                    else if (nightClub >= 7)
                        MissionProgress = 41;
                    else if (nightClub >= 6)
                        MissionProgress = 40;
                    else if (nightClub >= 5)
                        MissionProgress = 39;
                    else if (nightClub >= 4)
                        MissionProgress = 38;
                    else if (nightClub >= 3)
                        MissionProgress = 37;
                    else if (nightClub >= 2)
                        MissionProgress = 36;
                    else if (nightClub >= 1)
                        MissionProgress = 35;
                    nightClubProg = nightClub;
                }
                else if (gangWar != gangWarTBoGT)
                {
                    if (gangWar >= 25)
                        MissionProgress = 47;
                    else if (gangWar >= 20)
                        MissionProgress = 46;
                    else if (gangWar >= 15)
                        MissionProgress = 45;
                    else if (gangWar >= 10)
                        MissionProgress = 44;
                    else if (gangWar >= 5)
                        MissionProgress = 43;
                    gangWarTBoGT = gangWar;
                }
            }
        }
        private void MissionTracker()
        {
            if (currEp == 0)
            {
                float RomProg = GET_FLOAT_STAT(3);
                float VldProg = GET_FLOAT_STAT(4);
                float JacProg = GET_FLOAT_STAT(7);
                float FauProg = GET_FLOAT_STAT(8);
                float MnyProg = GET_FLOAT_STAT(9);
                float LizProg = GET_FLOAT_STAT(10);
                float DwaProg = GET_FLOAT_STAT(13);
                float BruProg = GET_FLOAT_STAT(16);
                float PBXProg = GET_FLOAT_STAT(17);
                float FrnProg = GET_FLOAT_STAT(18);
                float ULProg = GET_FLOAT_STAT(19);
                float PacProg = GET_FLOAT_STAT(22);
                float RayProg = GET_FLOAT_STAT(23);
                float GerProg = GET_FLOAT_STAT(24);
                float DerProg = GET_FLOAT_STAT(25);
                float BerProg = GET_FLOAT_STAT(26);
                float BelProg = GET_FLOAT_STAT(27);
                float GamProg = GET_FLOAT_STAT(28);
                float JmyProg = GET_FLOAT_STAT(29);

                if (RomProg != RomanProg)
                {
                    if (RomProg < 7)
                        MissionProgress = 1;
                    else if (RomProg < 14)
                        MissionProgress = 2;
                    else if (RomProg < 21)
                        MissionProgress = 3;
                    else if (RomProg < 27)
                        MissionProgress = 4;
                    else if (RomProg < 34)
                        MissionProgress = 5;
                    else if (RomProg < 41)
                        MissionProgress = 6;
                    else if (RomProg < 47)
                        MissionProgress = 12;
                    else if (RomProg < 54)
                        MissionProgress = 18;
                    else if (RomProg < 61)
                        MissionProgress = 24;
                    else if (RomProg < 67)
                        MissionProgress = 47;
                    else if (RomProg < 74)
                        MissionProgress = 64;
                    else if (RomProg < 81)
                        MissionProgress = 82;
                    else if (RomProg < 87)
                        MissionProgress = 83;
                    else if (RomProg < 94)
                        MissionProgress = 84;
                    else if (RomProg < 101)
                        MissionProgress = 85;
                    RomanProg = RomProg;
                }
                else if (VldProg != VladProg)
                {
                    if (VldProg < 30)
                        MissionProgress = 7;
                    else if (VldProg < 60)
                        MissionProgress = 8;
                    else if (VldProg < 80)
                        MissionProgress = 9;
                    else if (VldProg < 101)
                        MissionProgress = 10;
                    VladProg = VldProg;
                }
                else if (JacProg != JacobProg)
                {
                    if (JacProg < 60)
                        MissionProgress = 11;
                    else if (JacProg < 101)
                        MissionProgress = 15;
                    JacobProg = JacProg;
                }
                else if (FauProg != FaustProg)
                {
                    if (FauProg < 15)
                        MissionProgress = 13;
                    else if (FauProg < 30)
                        MissionProgress = 14;
                    else if (FauProg < 45)
                        MissionProgress = 16;
                    else if (FauProg < 60)
                        MissionProgress = 17;
                    else if (FauProg < 75)
                        MissionProgress = 19;
                    else if (FauProg < 90)
                        MissionProgress = 22;
                    else if (FauProg < 101)
                        MissionProgress = 23;
                    FaustProg = FauProg;
                }
                else if (BruProg != BrucieProg)
                {
                    if (BruProg < 30)
                        MissionProgress = 20;
                    else if (BruProg < 60)
                        MissionProgress = 21;
                    else if (BruProg < 80)
                        MissionProgress = 25;
                    else if (BruProg < 101)
                        MissionProgress = 26;
                    BrucieProg = BruProg;
                }
                else if (MnyProg != MannyProg)
                {
                    if (MnyProg < 40)
                        MissionProgress = 27;
                    else if (MnyProg < 70)
                        MissionProgress = 28;
                    else if (MnyProg < 101)
                        MissionProgress = 31;
                    MannyProg = MnyProg;
                }
                else if (LizProg != ElizProg)
                {
                    if (LizProg < 30)
                        MissionProgress = 29;
                    else if (LizProg < 60)
                        MissionProgress = 30;
                    else if (LizProg < 80)
                        MissionProgress = 32;
                    else if (LizProg < 101)
                        MissionProgress = 33;
                    ElizProg = LizProg;
                }
                else if (PBXProg != PBoyXProg)
                {
                    if (PBXProg < 40)
                        MissionProgress = 41;
                    else if (PBXProg < 70)
                        MissionProgress = 42;
                    else if (PBXProg < 101)
                        MissionProgress = 48;
                    PBoyXProg = PBXProg;
                }
                else if (DwaProg != DwayneProg)
                {
                    if (DwaProg < 60)
                        MissionProgress = 43;
                    else if (DwaProg < 101)
                        MissionProgress = 44;
                    DwayneProg = DwaProg;
                }
                else if (PacProg != PackieProg)
                {
                    if (PacProg < 40)
                        MissionProgress = 35;
                    else if (PacProg < 70)
                        MissionProgress = 36;
                    else if (PacProg < 101)
                        MissionProgress = 37;
                    PackieProg = PacProg;
                }
                else if (FrnProg != FrncsProg)
                {
                    if (FrnProg < 20)
                        MissionProgress = 34;
                    else if (FrnProg < 40)
                        MissionProgress = 38;
                    else if (FrnProg < 60)
                        MissionProgress = 39;
                    else if (FrnProg < 70)
                        MissionProgress = 40;
                    else if (FrnProg < 90)
                        MissionProgress = 57;
                    else if (FrnProg < 101)
                        MissionProgress = 58;
                    FrncsProg = FrnProg;
                }
                else if (ULProg != ULPProg)
                {
                    if (ULProg < 30)
                        MissionProgress = 45;
                    else if (ULProg < 60)
                        MissionProgress = 46;
                    else if (ULProg < 80)
                        MissionProgress = 49;
                    else if (ULProg < 101)
                        MissionProgress = 50;
                    ULPProg = ULProg;
                }
                else if (RayProg != RayBoProg)
                {
                    if (RayProg < 20)
                        MissionProgress = 54;
                    else if (RayProg < 40)
                        MissionProgress = 59;
                    else if (RayProg < 60)
                        MissionProgress = 60;
                    else if (RayProg < 70)
                        MissionProgress = 61;
                    else if (RayProg < 90)
                        MissionProgress = 62;
                    else if (RayProg < 101)
                        MissionProgress = 63;
                    RayBoProg = RayProg;
                }
                else if (GerProg != GerryProg)
                {
                    if (GerProg < 30)
                        MissionProgress = 51;
                    else if (GerProg < 50)
                        MissionProgress = 52;
                    else if (GerProg < 70)
                        MissionProgress = 65;
                    else if (GerProg < 90)
                        MissionProgress = 66;
                    else if (GerProg < 101)
                        MissionProgress = 70;
                    GerryProg = GerProg;
                }
                else if (DerProg != DerrickProg)
                {
                    if (DerProg < 40)
                        MissionProgress = 53;
                    else if (DerProg < 70)
                        MissionProgress = 55;
                    else if (DerProg < 101)
                        MissionProgress = 56;
                    DerrickProg = DerProg;
                }
                else if (BerProg != BernieProg)
                {
                    if (BerProg < 40)
                        MissionProgress = 67;
                    else if (BerProg < 70)
                        MissionProgress = 68;
                    else if (BerProg < 101)
                        MissionProgress = 69;
                    BernieProg = BerProg;
                }
                else if (BelProg != BellProg)
                {
                    if (BelProg < 40)
                        MissionProgress = 71;
                    else if (BelProg < 60)
                        MissionProgress = 74;
                    else if (BelProg < 80)
                        MissionProgress = 75;
                    else if (BelProg < 101)
                        MissionProgress = 76;
                    BellProg = BelProg;
                }
                else if (JmyProg != JimmyProg)
                {
                    if (JmyProg < 40)
                        MissionProgress = 72;
                    else if (JmyProg < 60)
                        MissionProgress = 73;
                    else if (JmyProg < 80)
                        MissionProgress = 77;
                    else if (JmyProg < 101)
                        MissionProgress = 78;
                    JimmyProg = JmyProg;
                }
                else if (GamProg != GambtiProg)
                {
                    if (GamProg < 40)
                        MissionProgress = 79;
                    else if (GamProg < 70)
                        MissionProgress = 80;
                    else if (GamProg < 101)
                        MissionProgress = 81;
                    GambtiProg = GamProg;
                }
            }
            else if (currEp == 1)
            {
                float BilProg = GET_FLOAT_STAT(121);
                float JmProg = GET_FLOAT_STAT(122);
                float StuProg = GET_FLOAT_STAT(125);
                float AshProg = GET_FLOAT_STAT(126);
                float LizProg = GET_FLOAT_STAT(127);
                float RayProg = GET_FLOAT_STAT(128);

                if (BilProg != BillyProg)
                {
                    if (BilProg < 20)
                        MissionProgress = 1;
                    else if (BilProg < 40)
                        MissionProgress = 2;
                    else if (BilProg < 60)
                        MissionProgress = 3;
                    else if (BilProg < 70)
                        MissionProgress = 4;
                    else if (BilProg < 90)
                        MissionProgress = 7;
                    else if (BilProg < 101)
                        MissionProgress = 11;
                    BillyProg = BilProg;
                }
                else if (JmProg != JimProg)
                {
                    if (JmProg < 30)
                        MissionProgress = 5;
                    else if (JmProg < 50)
                        MissionProgress = 6;
                    else if (JmProg < 70)
                        MissionProgress = 12;
                    else if (JmProg < 90)
                        MissionProgress = 13;
                    else if (JmProg < 101)
                        MissionProgress = 14;
                    JimProg = JmProg;
                }
                else if (StuProg != StubbsProg)
                {
                    if (StuProg < 40)
                        MissionProgress = 8;
                    else if (StuProg < 70)
                        MissionProgress = 10;
                    else if (StuProg < 101)
                        MissionProgress = 22;
                    StubbsProg = StuProg;
                }
                else if (AshProg != AshleyProg)
                {
                    if (AshProg < 70)
                        MissionProgress = 9;
                    else if (AshProg < 101)
                        MissionProgress = 18;
                    AshleyProg = AshProg;
                }
                else if (LizProg != LizTProg)
                {
                    if (LizProg < 40)
                        MissionProgress = 15;
                    else if (LizProg < 70)
                        MissionProgress = 16;
                    else if (LizProg < 101)
                        MissionProgress = 17;
                    LizTProg = LizProg;
                }
                else if (RayProg != RayBProg)
                {
                    if (RayProg < 40)
                        MissionProgress = 19;
                    else if (RayProg < 70)
                        MissionProgress = 20;
                    else if (RayProg < 101)
                        MissionProgress = 21;
                    RayBProg = RayProg;
                }
            }
            else if (currEp == 2)
            {
                float TonProg = GET_FLOAT_STAT(121);
                float YusProg = GET_FLOAT_STAT(122);
                float MorProg = GET_FLOAT_STAT(125);
                float ArmProg = GET_FLOAT_STAT(126);
                float BulProg = GET_FLOAT_STAT(127);
                float RocProg = GET_FLOAT_STAT(128);

                if (TonProg != TonyProg)
                {
                    if (TonProg < 10)
                        MissionProgress = 1;
                    else if (TonProg < 20)
                        MissionProgress = 2;
                    else if (TonProg < 30)
                        MissionProgress = 5;
                    else if (TonProg < 40)
                        MissionProgress = 6;
                    else if (TonProg < 50)
                        MissionProgress = 7;
                    else if (TonProg < 60)
                        MissionProgress = 8;
                    else if (TonProg < 70)
                        MissionProgress = 12;
                    else if (TonProg < 80)
                        MissionProgress = 14;
                    else if (TonProg < 90)
                        MissionProgress = 16;
                    else if (TonProg < 95)
                        MissionProgress = 22;
                    else if (TonProg < 101)
                        MissionProgress = 24;
                    TonyProg = TonProg;
                }
                else if (YusProg != YusufProg)
                {
                    if (YusProg < 30)
                        MissionProgress = 10;
                    else if (YusProg < 60)
                        MissionProgress = 11;
                    else if (YusProg < 90)
                        MissionProgress = 17;
                    else if (YusProg < 101)
                        MissionProgress = 20;
                    YusufProg = YusProg;
                }
                else if (MorProg != MoriProg)
                {
                    if (MorProg < 40)
                        MissionProgress = 9;
                    else if (MorProg < 70)
                        MissionProgress = 13;
                    else if (MorProg < 101)
                        MissionProgress = 15;
                    MoriProg = MorProg;
                }
                else if (ArmProg != ArmandoProg)
                {
                    if (ArmProg < 70)
                        MissionProgress = 3;
                    else if (ArmProg < 101)
                        MissionProgress = 4;
                    ArmandoProg = ArmProg;
                }
                else if (BulProg != BulgarinProg)
                {
                    if (BulProg < 40)
                        MissionProgress = 18;
                    else if (BulProg < 70)
                        MissionProgress = 19;
                    else if (BulProg < 101)
                        MissionProgress = 21;
                    BulgarinProg = BulProg;
                }
                else if (RocProg != RoccoProg)
                {
                    if (RocProg < 101)
                        MissionProgress = 23;
                    RoccoProg = RocProg;
                }
            }
            CheckWeaponUnlock(Settings);
        }
        private void AnimCheck()
        {
            if (isInspecting)
            {
                switch (animStart)
                {
                    case -1:
                        animTime = 0.5f;
                        break;
                    case 0:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "grenade_intro", out animTime);
                        break;
                    case 1:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "rifle_intro", out animTime);
                        break;
                    case 2:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "uzi_intro", out animTime);
                        break;
                    case 3:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "rpg_intro", out animTime);
                        break;
                    case 4:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "pistol_intro", out animTime);
                        break;
                    case 5:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "grenade_intro", out animTime);
                        break;
                    case 6:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "shotgun_intro", out animTime);
                        break;
                    case 7:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "pickup_low", out animTime);
                        break;
                    case 8:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "pickup_high", out animTime);
                        break;
                    case 9:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "pickup_med", out animTime);
                        break;
                }
            }
            else
            {
                switch (animEnd)
                {
                    case -1:
                        animTime = 0.5f;
                        break;
                    case 0:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "grenade_outro", out animTime);
                        break;
                    case 1:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "rifle_outro", out animTime);
                        break;
                    case 2:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "uzi_outro", out animTime);
                        break;
                    case 3:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "rpg_outro", out animTime);
                        break;
                    case 4:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "pistol_outro", out animTime);
                        break;
                    case 5:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "grenade_outro", out animTime);
                        break;
                    case 6:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "missgunlockup", "shotgun_outro", out animTime);
                        break;
                    case 7:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "putdown_low", out animTime);
                        break;
                    case 8:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "putdown_high", out animTime);
                        break;
                    case 9:
                        GET_CHAR_ANIM_CURRENT_TIME(Main.PlayerHandle, "pickup_object", "putdown_med", out animTime);
                        break;
                }
            }
        }
        private void CheckIfChanged()
        {
            if (weapChecked != weapID)
            {
                weapChecked = weapID;
                confirmation = false;
                canInspect = false;
            }
        }
        private void DespawnShit()
        {
            int i;
            for (i = 0; i < numOfWeaponIDs; i++)
            {
                if (!DOES_OBJECT_EXIST(WeaponProps[i]))
                    continue;

                DELETE_OBJECT(ref WeaponProps[i]);
            }

            for (i = 0; i < 4; i++)
            {
                if (DOES_CHAR_EXIST(GuardPeds[i]))
                    MARK_CHAR_AS_NO_LONGER_NEEDED(GuardPeds[i]);
            }
        }
        private void SpawnTheWeapon(int i)
        {
            if (!HAS_MODEL_LOADED(GET_HASH_KEY(weaponModel)))
                REQUEST_MODEL(GET_HASH_KEY(weaponModel));
            WeaponProps[i] = CreateObject_DontRequestModel(GET_HASH_KEY(weaponModel), 0.0f, 0.0f, 0.0f, 0.0f);
            SET_OBJECT_COORDINATES(WeaponProps[i], weapPos);
            SET_OBJECT_ROTATION(WeaponProps[i], weapRot);
            MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(weaponModel));

            //ADD_OBJECT_TO_INTERIOR_ROOM_BY_NAME(WeaponProps[i], "GtaMloRoom01");
            ADD_OBJECT_TO_INTERIOR_ROOM_BY_NAME(WeaponProps[i], roomName);

            FREEZE_OBJECT_POSITION(WeaponProps[i], true);
        }
        private void SpawnGuns()
        {
            if (LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 40, 40, 40, false) && !HasSpawned && !IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
            {
                RequestAnims();
                SET_STATE_OF_CLOSEST_DOOR_OF_TYPE((uint)doorHash, doorPos.X, doorPos.Y, doorPos.Z, false, 0.0f);
                HasSpawned = true;

                int i;
                for (i = 0; i < numOfWeaponIDs; i++)
                {
                    if (DOES_OBJECT_EXIST(WeaponProps[i]))
                        continue;

                }
                for (i = 0; i < numOfWeaponIDs; i++)
                {
                    if (episodicCheck)
                    {
                        if (currEp == 0 && i > 20 && i < 41)
                            continue;

                        else if (currEp == 1 && i > 28 && i < 41)
                            continue;

                        else if (currEp == 2 && i > 21 && i < 29)
                            continue;
                    }
                    LoadWeaponPosition(Settings, i);
                }
                backroomCount = backroomWeaps.Count();
            }
            else if (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
                HasSpawned = false;

            else if (!LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 40, 40, 40, false))
            {
                DespawnShit();
                HasSpawned = false;
                bSpawnDealer = false;
            }
        }
        private void BuyGuns()
        {
            if (!HasSpawned)
                return;

            float x, y, z;
            GET_CHAR_COORDINATES(Main.PlayerHandle, out x, out y, out z);

            int i;
            if (!bIsHostile)
            {
                for (i = 0; i < numOfWeaponIDs; i++)
                {
                    if (!DOES_OBJECT_EXIST(WeaponProps[i]))
                        continue;

                    GET_OBJECT_COORDINATES(WeaponProps[i], out weapCoords[i].X, out weapCoords[i].Y, out weapCoords[i].Z);
                    LoadWeaponBuyPos(Settings, i);
                    if (LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, weapBuyPos.X, weapBuyPos.Y, weapBuyPos.Z, weapBuyRad, weapBuyRad, weapBuyRad, showBuyMarkers ? true : false) || LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, backroomPos.X, backroomPos.Y, backroomPos.Z, 1, 1, 1, true))
                    {
                        if (!LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, backroomPos.X, backroomPos.Y, backroomPos.Z, 1, 1, 1, true))
                        {
                            GetWeaponData(Settings, i);
                            weapID = i;
                            weapPellets = (int)IVWeaponInfo.GetWeaponInfo((uint)i).AimingPellets;
                            CheckIfChanged();
                            wCoords = weapBuyPos;
                            wRad = weapBuyRad;
                            if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, i)))
                            {
                                if (!confirmation && !bInspectMsg && !bFailMsg && !menuActive)
                                {
                                    if (!HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, i))
                                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST2", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weaponName + " $" + weapCost.ToString());
                                    else
                                    {
                                            if (IVWeaponInfo.GetWeaponInfo((uint)i).WeaponSlot <= 1 || IVWeaponInfo.GetWeaponInfo((uint)i).WeaponSlot > 7)
                                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST2", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weapStatE.ToString() + "x " + weaponName + "s $" + ammoCost.ToString());
                                            else
                                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST2", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + clipAmmo.ToString() + "x " + weaponName + " Ammo $" + ammoCost.ToString());
                                    }

                                    PRINT_HELP_FOREVER("GL_PIST2");
                                }
                                canInspect = true;
                            }
                            else if (!IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON"))
                                PRINT_HELP_FOREVER("GL_SOON");
                        }
                        else
                        {
                            if (backroomCount > 0)
                            {
                                weapID = backroomWeaps[backroomSelectedWeap];
                                if (!menuActive)
                                    isWeapUnlocked = true;
                                else
                                    GetWeaponData(Settings, weapID);
                                weapPellets = (int)IVWeaponInfo.GetWeaponInfo((uint)i).AimingPellets;
                                CheckIfChanged();
                                if (!confirmation && !bInspectMsg && !bFailMsg)
                                {
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST2", "~s~Press ~INPUT_PICKUP~ to browse extra weapons.");
                                    PRINT_HELP_FOREVER("GL_PIST2");
                                }
                                if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID)))
                                    canBuyStock = true;
                                else if (!IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON"))
                                {
                                    canBuyStock = false;
                                    PRINT_HELP_FOREVER("GL_SOON");
                                }
                                canInspect = true;
                                extraWeap = true;
                            }
                            else
                            {
                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST2", "~r~There are no extra weapons in stock.");
                                PRINT_HELP_FOREVER("GL_PIST2");
                            }
                        }
                        break;
                    }
                    else if (!LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, wRad, wRad, wRad, false) && !LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, backroomPos.X, backroomPos.Y, backroomPos.Z, 1, 1, 1, false))
                    {
                        if (IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_PIST2") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GLOCK_WH2"))
                            CLEAR_HELP();
                        backroomSelectedWeap = 0;
                        extraWeap = false;
                        confirmation = false;
                        canInspect = false;
                        attachmentMenu = false;
                    }
                }
            }
            else
            {
                if (IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_PIST2") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GLOCK_WH2"))
                    CLEAR_HELP();
                backroomSelectedWeap = 0;
                extraWeap = false;
                confirmation = false;
                canInspect = false;
                attachmentMenu = false;
            }
        }
        private void ProcessAttachments()
        {
            if (!attachmentMenu)
                return;

            //IVGame.ShowSubtitleMessage(currAttachmt.ToString());
            //IVGame.ShowSubtitleMessage(attachmentsConfig.GetBoolean(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "HasGLAttachment", false).ToString() + "  " + weapFuncsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "GrenadeAmmo", 0).ToString());
            //IVGame.ShowSubtitleMessage(attachmentsConfig.GetBoolean(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "HasGLAttachment", false).ToString());
        }
        private void ProcessInspection()
        {
            if (!canInspect)
                return;

            if (!isInspecting && !camActivate)
            {
                SET_PLAYER_CONTROL(Main.PlayerIndex, false);
                CLEAR_CHAR_TASKS(Main.PlayerHandle);

                //_TASK_CHAR_SLIDE_TO_COORD(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, plyrHeading, 1.0f);
                //WAIT(1000);
                _TASK_GO_STRAIGHT_TO_COORD(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, 1, 45000);
                goInspect = true;
            }
            else if (!camActivate)
                goInspect = true;
        }
        private void DestroyCamera()
        {
            SET_CAM_ACTIVE(camera, false);
            SET_CAM_PROPAGATE(camera, false);
            DESTROY_CAM(camera);
            ACTIVATE_SCRIPTED_CAMS(false, false);
            SET_CAM_BEHIND_PED(Main.PlayerHandle);
            SET_GAME_CAM_PITCH(0.0f);
            SET_GAME_CAM_HEADING(0.0f);
            DISPLAY_HUD(true);
            DISPLAY_RADAR(true);
        }
        private void ProcessInspectionCam()
        {
            if (!extraWeap)
            {
                if (animTime > 0.5)
                {
                    if (isInspecting)
                    {
                        if (DOES_OBJECT_EXIST(WeaponProps[weapID]))
                            SET_OBJECT_VISIBLE(WeaponProps[weapID], false);

                        CreateCamera(camera, true, 0.9f, -0.3f, 1.1f, weapCoords[weapID], 60);

                        if (weapInSlot != weapID)
                        {
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapInSlot);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, clipAmmo, true);
                        }

                        SET_CURRENT_CHAR_WEAPON(Main.PlayerHandle, weapID, true);
                        menuActive = true;
                    }

                    else
                    {
                        int i;
                        DestroyCamera();
                        SET_PLAYER_CONTROL(Main.PlayerIndex, true);

                        if (weapInSlot > 0)
                        {
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapID);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapInSlot, ammoInSlot, false);
                        }
                        else
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapID);

                        for (i = 0; i < numOfWeaponIDs; i++)
                        {
                            if (!DOES_OBJECT_EXIST(WeaponProps[i]))
                                continue;

                            SET_OBJECT_VISIBLE(WeaponProps[i], true);
                        }
                        menuActive = false;
                    }
                    camActivate = false;
                }
            }
            else
            {
                if (isInspecting)
                {
                    GET_OFFSET_FROM_CHAR_IN_WORLD_COORDS(Main.PlayerHandle, new Vector3(0, 3, 0.2f), out Vector3 pOff);
                    //CreateCamera(camera, true, 0.0f, 1.75f, 0.0f, pOff, 60);
                    CreateCamera(camera, true, 0.0f, 0.5f, 0.2f, pOff, 60);
                    spawnWeaponInAir = false;
                    menuActive = true;
                    DeleteBackroomWeapon();
                }
                else
                {
                    DestroyCamera();
                    SET_PLAYER_CONTROL(Main.PlayerIndex, true);
                    spawnWeaponInAir = false;
                    menuActive = false;
                    DeleteBackroomWeapon();
                }
                camActivate = false;
            }
        }
        private void ProcessInspectAnims()
        {
            if (!extraWeap)
            {
                if (!isInspecting)
                {
                    if (LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, 1.5f, 1.5f, 1.5f, false))
                    {
                        SET_CHAR_COORDINATES(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z - 1.0f);
                        SET_CHAR_HEADING(Main.PlayerHandle, weapBuyHdg);
                        GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                        GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);

                        switch (animStart)
                        {
                            case -1:
                                break;
                            case 0:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 1:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rifle_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 2:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "uzi_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 3:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rpg_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 4:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pistol_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 5:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 6:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "shotgun_intro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 7:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pickup_low", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 8:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pickup_high", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                                break;
                            case 9:
                                _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pickup_med", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                                break;
                        }
                        camActivate = true;
                        goInspect = false;
                        isInspecting = true;
                    }
                }

                else if (isInspecting)
                {
                    switch (animEnd)
                    {
                        case -1:
                            break;
                        case 0:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, 0);
                            break;
                        case 1:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rifle_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, 0);
                            break;
                        case 2:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "uzi_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 3:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rpg_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 4:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pistol_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 5:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 6:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "shotgun_outro", "missgunlockup", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 7:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "putdown_low", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 8:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "putdown_high", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                            break;
                        case 9:
                            _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "putdown_med", "pickup_object", 4.0f, 0, 0, 0, 0, -1);
                            break;
                    }

                    camActivate = true;
                    goInspect = false;
                    isInspecting = false;
                    bComparing = false;
                    bInspectMsg = false;
                    CLEAR_CHAR_TASKS(Main.PlayerHandle);
                }
            }
            else
            {
                if (!isInspecting)
                {
                    if (LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, 1.5f, 1.5f, 1.5f, false))
                    {
                        CLEAR_CHAR_TASKS_IMMEDIATELY(Main.PlayerHandle);
                        SET_CHAR_COORDINATES(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z - 1.0f);
                        SET_CHAR_HEADING(Main.PlayerHandle, weapBuyHdg);
                            GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                            GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);
                        camActivate = true;
                        goInspect = false;
                        isInspecting = true;
                    }
                }
                else if (isInspecting)
                {
                    camActivate = true;
                    goInspect = false;
                    isInspecting = false;
                    bComparing = false;
                    bInspectMsg = false;
                    CLEAR_CHAR_TASKS(Main.PlayerHandle);
                }
            }
        }
        private void SpawnBackroomWeapon()
        {
            if (spawnWeaponInAir)
                return;

            if (DOES_OBJECT_EXIST(weapInAir))
                DELETE_OBJECT(ref weapInAir);

            if (currEp == 0)
                weaponModel = Settings.GetValue(weapID.ToString(), "IVWeaponModel", "");
            else if (currEp == 1)
                weaponModel = Settings.GetValue(weapID.ToString(), "TLADWeaponModel", "");
            else if (currEp == 2)
                weaponModel = Settings.GetValue(weapID.ToString(), "TBOGTWeaponModel", "");

            REQUEST_MODEL(GET_HASH_KEY(weaponModel));
            if (!HAS_MODEL_LOADED(GET_HASH_KEY(weaponModel)))
                return;

            weapInAir = CreateObject_DontRequestModel(GET_HASH_KEY(weaponModel), backroomPos.X, backroomPos.Y, backroomPos.Z, backroomHdg);

            Vector3 wOffset = Settings.GetVector3(weapID.ToString(), "BackroomOffset", Vector3.Zero);
            GET_OFFSET_FROM_OBJECT_IN_WORLD_COORDS(weapInAir, wOffset, out Vector3 objOff);
            SET_OBJECT_COORDINATES(weapInAir, objOff);
            MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(weaponModel));

            ADD_OBJECT_TO_INTERIOR_ROOM_BY_NAME(weapInAir, roomName);

            FREEZE_OBJECT_POSITION(weapInAir, true);
            spawnWeaponInAir = true;
        }
        private void DeleteBackroomWeapon()
        {
            if (DOES_OBJECT_EXIST(weapInAir))
                DELETE_OBJECT(ref weapInAir);
        }
        private void HelpDisplay()
        {
            if (!bInspectMsg || (extraWeap && !confirmation && !bFailMsg) || (attachmentMenu && !confirmation && !bFailMsg))
            {
                if (!extraWeap)
                {
                    if (!attachmentMenu)
                    {
                        /*if (hasGLAttachment && weapID == weapInSlot)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Press ~INPUT_PICKUP~ to stop inspecting. ~n~~s~Press ~INPUT_JUMP~ to browse attachments.");*/
                        if (weapInSlot <= 0 || weapInSlot == weapID)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Press ~INPUT_PICKUP~ to stop inspecting.");
                        else
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Press ~INPUT_PICKUP~ to stop inspecting. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to compare with current weapon in the same slot.");
                        for (int i = 0; i < numOfAttachmts; i++)
                        {
                            if (hasAttachment[i] && weapID == weapInSlot)
                            {
                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Press ~INPUT_PICKUP~ to stop inspecting. ~n~~s~Press ~INPUT_JUMP~ to browse attachments.");
                                break;
                            }
                        }
                    }
                    else
                    {
                        weapFuncsConfig.Load();
                        grenAmmo = weapFuncsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "GrenadeAmmo", 0);
                        switch (currAttachmt)
                        {
                            case 0:
                                if (!ownsAttachment[0])
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Grenade Launcher attachment $" + attachmentPrice.ToString());
                                else
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~g~$" + grenPrice.ToString() + "~n~~s~Currently have ~g~" + grenAmmo.ToString() + "x ~s~ammo.");
                                break;
                            case 1:
                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Scope attachment $" + attachmentPrice.ToString());
                                break;
                        }
                    }
                }
                else
                {
                    GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                    GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);
                    /*GET_WEAPONTYPE_MODEL(weapID, out uint wModel);
                    string wString = GET_STRING_FROM_HASH_KEY(wModel);

                    int txd = 0;
                    txd = GET_TXD(wString);

                    if (!HAS_STREAMED_TXD_LOADED(wString))
                        LOAD_TXD(wString);

                    int tex = 0;
                    tex = GET_TEXTURE(txd, wString);

                    DRAW_SPRITE((uint)tex, 0.1f, 0.3f, 0.15f, 0.15f, 0, 255, 255, 255, 255);*/
                    if (!attachmentMenu)
                    {
                        SpawnBackroomWeapon();

                        if (weapInSlot <= 0)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponName + " $" + weapCost.ToString());
                        /*else if (hasGLAttachment && weapID == weapInSlot)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~PAD_UP~ to browse attachments. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + clipAmmo.ToString() + "x " + weaponName + " Ammo $" + ammoCost.ToString() + "~n~~s~Currently have ~g~" + ammoInSlot.ToString() + "x ~s~ammo.");*/
                        else if (weapInSlot == weapID)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + clipAmmo.ToString() + "x " + weaponName + " Ammo $" + ammoCost.ToString() + "~n~~s~Currently have ~g~" + ammoInSlot.ToString() + "x ~s~ammo.");
                        else
                            IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_JUMP~ to compare with current weapon in the same slot. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponName + " $" + weapCost.ToString());
                    }
                    else
                    {
                        weapFuncsConfig.Load();
                        grenAmmo = weapFuncsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "GrenadeAmmo", 0);
                        switch (currAttachmt)
                        {
                            case 0:
                                if (!ownsAttachment[0])
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Grenade Launcher attachment. $" + attachmentPrice.ToString());
                                else
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. $" + grenPrice.ToString() + "~n~~s~Currently have ~g~" + grenAmmo.ToString() + "x ~s~ammo.");
                                break;
                            case 1:
                                IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL3", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Scope attachment $" + attachmentPrice.ToString());
                                break;
                        }
                    }
                }
                if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID)))
                    PRINT_HELP_FOREVER("GL_RKL3");

                bInspectMsg = true;
            }
        }
        private void ProcessStatDisplay()
        {
            if (isInspecting)
            {
                DISPLAY_AMMO(false);
                DRAW_RECT(0.85f, 0.5f, 0.3f, 1, 20, 20, 20, 128);
                DRAW_RECT(0.85f, 0.085f, 0.3f, 0.17f, 0, 0, 0, 255);

                SET_TEXT_SCALE(0.5f, 0.5f);
                SET_TEXT_FONT(6);
                SET_TEXT_COLOUR((uint)hudR, (uint)hudG, (uint)hudB, 255);
                SET_TEXT_CENTRE(true);

                //Weapon Names
                IVText.TheIVText.ReplaceTextOfTextLabel("GL_PIST4", weaponName);
                DISPLAY_TEXT(0.85f, 0.07f, "GL_PIST4");

                //Weapon Stats Text
                if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID)))
                {
                    SET_TEXT_SCALE(0.3f, 0.3f);
                    SET_TEXT_FONT(6);
                    SET_TEXT_COLOUR((uint)hudTextR, (uint)hudTextG, (uint)hudTextB, 255);
                    SET_TEXT_WRAP(0.725f, 0.975f);
                    SET_TEXT_DROPSHADOW(false, 0, 0, 0, 0);

                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_DEAGLE2", "Slot");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_DEAGLE3", "x~1~");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_DEAGLE4", "Melee");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_GREN2", "Handguns");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_GREN3", "Shotguns");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_GREN4", "SMGs");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_M40A12", "Assault Rifles");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_M40A13", "Sniper Rifles");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_M40A14", "Heavy");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_M42", "Thrown");
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_M43", "Special");

                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_AK473", statTextA);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_AK474", statTextB);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BARET2", statTextC);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BARET3", statTextD);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BARET4", statTextE);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BBAT2", statTextF);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BBAT3", statTextG);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GL_BBAT4", statTextH);

                    DISPLAY_TEXT(0.725f, 0.19f, "GL_DEAGLE2");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.23f, "GL_AK473");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.27f, "GL_AK474");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.31f, "GL_BARET2");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.35f, "GL_BARET3");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.39f, "GL_BARET4");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.43f, "GL_BBAT2");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.47f, "GL_BBAT3");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.51f, "GL_BBAT4");

                    //Weapon Slots & Numerical Info
                    if (weapStatA >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.23f, "NUMBER", weapStatA);

                        if (weapSlot == 3 && weapPellets != 1)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            if (weapPellets > 1)
                                DISPLAY_TEXT_WITH_NUMBER(0.958f, 0.23f, "GL_DEAGLE3", weapPellets + 2);
                            else
                                DISPLAY_TEXT_WITH_NUMBER(0.958f, 0.23f, "GL_DEAGLE3", 18);
                        }
                    }

                    if (weapStatB >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.27f, "NUMBER", weapStatB);
                    }

                    if (weapStatC >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.31f, "NUMBER", weapStatC);
                    }

                    if (weapStatD >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.35f, "NUMBER", weapStatD);
                    }

                    if (weapStatE >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.39f, "NUMBER", weapStatE);
                    }

                    if (weapStatF >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.43f, "NUMBER", weapStatF);
                    }

                    if (weapStatG >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.47f, "NUMBER", weapStatG);
                    }

                    if (weapStatH >= 0)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.51f, "NUMBER", weapStatH);
                    }

                    USE_PREVIOUS_FONT_SETTINGS();
                    SET_TEXT_COLOUR(255, 255, 255, 255);
                    if (weapSlot == 1)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_DEAGLE4");
                    else if (weapSlot == 2)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_GREN2");
                    else if (weapSlot == 3)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_GREN3");
                    else if (weapSlot == 4)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_GREN4");
                    else if (weapSlot == 5)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_M40A12");
                    else if (weapSlot == 6)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_M40A13");
                    else if (weapSlot == 7)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_M40A14");
                    else if (weapSlot == 8)
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_M42");
                    else
                        DISPLAY_TEXT(0.83f, 0.19f, "GL_M43");

                    //Weapon Bars
                    float damageSize = weapBarA / 1000.0f;
                    float accuracySize = weapBarB / 1000.0f;
                    float rangeSize = weapBarC / 1000.0f;
                    float RoFSize = weapBarD / 1000.0f;
                    float magSize = weapBarE / 1000.0f;
                    float ammoSize = weapBarF / 1000.0f;
                    float aimSize = weapBarG / 1000.0f;
                    float forceSize = weapBarH / 1000.0f;

                    float damagePos = 0.882f - (0.1f - damageSize) / 2;
                    float accuracyPos = 0.882f - (0.1f - accuracySize) / 2;
                    float rangePos = 0.882f - (0.1f - rangeSize) / 2;
                    float RoFPos = 0.882f - (0.1f - RoFSize) / 2;
                    float magPos = 0.882f - (0.1f - magSize) / 2;
                    float ammoPos = 0.882f - (0.1f - ammoSize) / 2;
                    float aimPos = 0.882f - (0.1f - aimSize) / 2;
                    float forcePos = 0.882f - (0.1f - forceSize) / 2;

                    if (weapBarA >= 0)
                    {
                        DRAW_RECT(0.882f, 0.24f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(damagePos, 0.24f, damageSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarB >= 0)
                    {
                        DRAW_RECT(0.882f, 0.28f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(accuracyPos, 0.28f, accuracySize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarC >= 0)
                    {
                        DRAW_RECT(0.882f, 0.32f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(rangePos, 0.32f, rangeSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarD >= 0)
                    {
                        DRAW_RECT(0.882f, 0.36f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(RoFPos, 0.36f, RoFSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarE >= 0)
                    {
                        DRAW_RECT(0.882f, 0.40f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(magPos, 0.40f, magSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarF >= 0)
                    {
                        DRAW_RECT(0.882f, 0.44f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(ammoPos, 0.44f, ammoSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarG >= 0)
                    {
                        DRAW_RECT(0.882f, 0.48f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(aimPos, 0.48f, aimSize, 0.015f, 255, 255, 255, 255);
                    }
                    if (weapBarH >= 0)
                    {
                        DRAW_RECT(0.882f, 0.52f, 0.1f, 0.015f, 50, 50, 50, 255);
                        DRAW_RECT(forcePos, 0.52f, forceSize, 0.015f, 255, 255, 255, 255);
                    }

                    //Weapon Messages
                    if (!bComparing)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL4", weapMsg);
                        DISPLAY_TEXT(0.725f, 0.55f, "GL_RKL4");
                    }
                    //Comparing weapons
                    else
                    {
                        GetCurrWeaponData(Settings, weapInSlot);
                        SET_TEXT_SCALE(0.5f, 0.5f);
                        SET_TEXT_FONT(6);
                        SET_TEXT_COLOUR((uint)hudR, (uint)hudG, (uint)hudB, 255);
                        SET_TEXT_CENTRE(true);

                        //Weapon Names
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_AK472", pWeaponName);
                        DISPLAY_TEXT(0.85f, 0.57f, "GL_AK472");

                        //Weapon Stats Text
                        SET_TEXT_SCALE(0.3f, 0.3f);
                        SET_TEXT_FONT(6);
                        SET_TEXT_COLOUR((uint)hudTextR, (uint)hudTextG, (uint)hudTextB, 255);
                        SET_TEXT_WRAP(0.725f, 0.975f);
                        SET_TEXT_DROPSHADOW(false, 0, 0, 0, 0);

                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_M44", pStatTextA);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MOLOTOV2", pStatTextB);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MOLOTOV3", pStatTextC);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MOLOTOV4", pStatTextD);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MP52", pStatTextE);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MP53", pStatTextF);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_MP54", pStatTextG);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GL_RKL2", pStatTextH);

                        DISPLAY_TEXT(0.725f, 0.64f, "GL_M44");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.68f, "GL_MOLOTOV2");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.72f, "GL_MOLOTOV3");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.76f, "GL_MOLOTOV4");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.80f, "GL_MP52");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.84f, "GL_MP53");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.88f, "GL_MP54");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.92f, "GL_RKL2");

                        //Weapon Slots & Numerical Info
                        if (pWeapStatA >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.64f, "NUMBER", pWeapStatA);

                            pWeapPellets = (int)IVWeaponInfo.GetWeaponInfo((uint)weapInSlot).AimingPellets;

                            if (weapSlot == 3 && pWeapPellets != 1)
                            {
                                USE_PREVIOUS_FONT_SETTINGS();
                                if (pWeapPellets > 1)
                                    DISPLAY_TEXT_WITH_NUMBER(0.958f, 0.64f, "GL_DEAGLE3", pWeapPellets + 2);
                                else
                                    DISPLAY_TEXT_WITH_NUMBER(0.958f, 0.64f, "GL_DEAGLE3", 18);
                            }
                        }
                        if (pWeapStatB >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.68f, "NUMBER", pWeapStatB);
                        }
                        if (pWeapStatC >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.72f, "NUMBER", pWeapStatC);
                        }
                        if (pWeapStatD >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.76f, "NUMBER", pWeapStatD);
                        }
                        if (pWeapStatE >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.80f, "NUMBER", pWeapStatE);
                        }
                        if (pWeapStatF >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.84f, "NUMBER", pWeapStatF);
                        }
                        if (pWeapStatG >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.88f, "NUMBER", pWeapStatG);
                        }
                        if (pWeapStatH >= 0)
                        {
                            USE_PREVIOUS_FONT_SETTINGS();
                            DISPLAY_TEXT_WITH_NUMBER(0.94f, 0.92f, "NUMBER", pWeapStatH);
                        }

                        float pDamageSize = pWeapBarA / 1000.0f;
                        float pAccuracySize = pWeapBarB / 1000.0f;
                        float pRangeSize = pWeapBarC / 1000.0f;
                        float pRoFSize = pWeapBarD / 1000.0f;
                        float pMagSize = pWeapBarE / 1000.0f;
                        float pAmmoSize = pWeapBarF / 1000.0f;
                        float pAimSize = pWeapBarG / 1000.0f;
                        float pForceSize = pWeapBarH / 1000.0f;

                        float pDamagePos = 0.882f - (0.1f - pDamageSize) / 2;
                        float pAccuracyPos = 0.882f - (0.1f - pAccuracySize) / 2;
                        float pRangePos = 0.882f - (0.1f - pRangeSize) / 2;
                        float pRoFPos = 0.882f - (0.1f - pRoFSize) / 2;
                        float pMagPos = 0.882f - (0.1f - pMagSize) / 2;
                        float pAmmoPos = 0.882f - (0.1f - pAmmoSize) / 2;
                        float pAimPos = 0.882f - (0.1f - pAimSize) / 2;
                        float pForcePos = 0.882f - (0.1f - pForceSize) / 2;

                        //Weapon Bars
                        if (pWeapBarA >= 0)
                        {
                            DRAW_RECT(0.882f, 0.65f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pDamagePos, 0.65f, pDamageSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarB >= 0)
                        {
                            DRAW_RECT(0.882f, 0.69f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pAccuracyPos, 0.69f, pAccuracySize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarC >= 0)
                        {
                            DRAW_RECT(0.882f, 0.73f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pRangePos, 0.73f, pRangeSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarD >= 0)
                        {
                            DRAW_RECT(0.882f, 0.77f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pRoFPos, 0.77f, pRoFSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarE >= 0)
                        {
                            DRAW_RECT(0.882f, 0.81f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pMagPos, 0.81f, pMagSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarF >= 0)
                        {
                            DRAW_RECT(0.882f, 0.85f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pAmmoPos, 0.85f, pAmmoSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarG >= 0)
                        {
                            DRAW_RECT(0.882f, 0.89f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pAimPos, 0.89f, pAimSize, 0.015f, 255, 255, 255, 255);
                        }
                        if (pWeapBarH >= 0)
                        {
                            DRAW_RECT(0.882f, 0.93f, 0.1f, 0.015f, 50, 50, 50, 255);
                            DRAW_RECT(pForcePos, 0.93f, pForceSize, 0.015f, 255, 255, 255, 255);
                        }
                    }
                }
                HelpDisplay();

                if (animTime > 0.5)
                {
                    if (animLoop == 0 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "over_shoulder"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "over_shoulder", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 1 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "rifle_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rifle_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 2 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "uzi_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "uzi_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 3 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "rpg_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rpg_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 4 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "pistol_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pistol_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 5 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "grenade_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (animLoop == 6 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "shotgun_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "shotgun_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                }
            }
        }
        private void BuyTheWeapon()
        {
            if (!extraWeap)
            {
                GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);
            }
            GET_CHAR_ARMOUR(Main.PlayerHandle, out uint pArmr);

            if (pAmmo < maxAmmo)
            {
                if (HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID) && pMoney >= ammoCost)
                {
                    if (weapID != 48)
                    {
                        PLAY_SOUND(soundID, buySound);
                        ADD_SCORE(Main.PlayerIndex, -ammoCost);
                        SAY_AMBIENT_SPEECH(GuardPeds[0], "THANKS", false, true, 0);
                        GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, clipAmmo, true);
                    }
                    else if (pArmr < 100)
                    {
                        PLAY_SOUND(soundID, buySound);
                        ADD_SCORE(Main.PlayerIndex, -ammoCost);
                        SAY_AMBIENT_SPEECH(GuardPeds[0], "THANKS", false, true, 0);
                        ADD_ARMOUR_TO_CHAR(Main.PlayerHandle, 100);
                    }
                }
                else if (pMoney >= weapCost)
                {
                    if (weapInSlot > 0 && weapInSlot != weapID && !confirmation)
                    {
                        confirmation = true;
                        PRINT_HELP("GLOCK_WH2");
                    }
                    else
                    {
                        bComparing = false;
                        confirmation = false;
                        if (weapID != 48)
                        {
                            PLAY_SOUND(soundID, buySound);
                            ADD_SCORE(Main.PlayerIndex, -weapCost);
                            //PLAY_AUDIO_EVENT_FROM_PED("GS1_BUY", GuardPeds[0]);
                            //int b = GET_HASH_KEY("GUN_NUT");
                            //NEW_SCRIPTED_CONVERSATION();
                            //ADD_NEW_FRONTEND_CONVERSATION_SPEAKER(GuardPeds[0], b);
                            //ADD_NEW_CONVERSATION_SPEAKER(0, GuardPeds[0], out b);
                            //ADD_LINE_TO_CONVERSATION(0, GET_HASH_KEY("GS1_MA"), GET_HASH_KEY("GS1_PURCH2"), 0, 0);
                            //START_SCRIPT_CONVERSATION(true, true);
                            //IVGame.ShowSubtitleMessage(b.ToString() + "   " + GET_HASH_KEY("GS1_MA") + "   " + unchecked((int)2484359393));
                            SAY_AMBIENT_SPEECH(GuardPeds[0], "THANKS", false, true, 0);
                            if (!keepAmmo)
                                REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapInSlot);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, clipAmmo, true);
                        }
                        else if (pArmr < 100)
                        {
                            PLAY_SOUND(soundID, buySound);
                            ADD_SCORE(Main.PlayerIndex, -weapCost);
                            SAY_AMBIENT_SPEECH(GuardPeds[0], "THANKS", false, true, 0);
                            ADD_ARMOUR_TO_CHAR(Main.PlayerHandle, 100);
                        }
                    }
                }
                else
                {
                    IVText.TheIVText.ReplaceTextOfTextLabel("GUN_FAIL", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~r~You don't have enough cash for that weapon.");
                    PRINT_HELP("GUN_FAIL");
                    GET_GAME_TIMER(out fTimer);
                    bFailMsg = true;
                }
            }
            else
            {
                IVText.TheIVText.ReplaceTextOfTextLabel("AMMO_FULL", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~n~~r~You can't carry more of that type of ammo.");
                PRINT_HELP("AMMO_FULL");
                GET_GAME_TIMER(out fTimer);
                bFailMsg = true;
            }
        }
        private void BuyAttachment()
        {
            if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
            {
                weapFuncsConfig.Load();
                grenAmmo = weapFuncsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, weapID.ToString() + "GrenadeAmmo", 0);

                if (!ownsAttachment[currAttachmt])
                {
                    if (pMoney >= attachmentPrice)
                    {
                        PLAY_SOUND(soundID, buySound);
                        //WriteBooleanToINI(attachmentsConfig, weapID.ToString() + "HasGLAttachment", true);
                        attachmentUnlocks[weapID][currAttachmt] = true;
                        WriteBooleanToINI(weapFuncsConfig, weapID.ToString() + "Has" + GetAttachmentName(currAttachmt) + "Attachment", true);
                        ADD_SCORE(Main.PlayerIndex, -attachmentPrice);
                        if (currAttachmt == 0 && grenAmmo < attachmentsConfig.GetInteger(weapID.ToString(), "MaxGrenadeAmmo", 0))
                            WriteIntToINI(weapFuncsConfig, weapID.ToString() + "GrenadeAmmo", grenAmmo + 1);
                        weapFuncsConfig.Save();
                    }
                }
                else if (currAttachmt == 0 && pMoney >= grenPrice && grenAmmo < attachmentsConfig.GetInteger(weapID.ToString(), "MaxGrenadeAmmo", 0))
                {
                    ADD_SCORE(Main.PlayerIndex, -grenPrice);
                    WriteIntToINI(weapFuncsConfig, weapID.ToString() + "GrenadeAmmo", grenAmmo + 1);
                    weapFuncsConfig.Save();
                }
                else if (currAttachmt == 0 && grenAmmo >= attachmentsConfig.GetInteger(weapID.ToString(), "MaxGrenadeAmmo", 0))
                {
                    IVText.TheIVText.ReplaceTextOfTextLabel("AMMO_FULL", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~n~~r~You can't carry more of that type of ammo.");
                    PRINT_HELP("AMMO_FULL");
                    GET_GAME_TIMER(out fTimer);
                    bFailMsg = true;
                }
            }
        }
        private void ProcessPurchase()
        {
            if (!HasSpawned)
                return;

            if (!canInspect)
                return;

            GetWeaponData(Settings, weapID);
            STORE_SCORE(Main.PlayerIndex, out pMoney);
            GET_AMMO_IN_CHAR_WEAPON(Main.PlayerHandle, weapID, out pAmmo);
            GET_MAX_AMMO(Main.PlayerHandle, weapID, out maxAmmo);
            //GET_MAX_AMMO_IN_CLIP(Main.PlayerHandle, weapID, out clipAmmo);

            //Inspect anim loop and stats display
            if (menuActive)
                ProcessStatDisplay();

            if (!bFailMsg)
            {
                if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.Action) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.Action)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (confirmation)
                        confirmation = false;
                    else if (attachmentMenu)
                    {
                        attachmentMenu = false;
                        bInspectMsg = false;
                    }
                    else if (!extraWeap && weapID != 48)
                        ProcessInspection();
                    else
                    {
                        backroomSelectedWeap = 0;
                        wCoords = backroomPos;
                        weapBuyHdg = backroomHdg;

                        ProcessInspection();
                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.Action) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.Action))
                {
                    if (!extraWeap)
                    {
                        if (!isInspecting && !menuActive && !attachmentMenu)
                            BuyTheWeapon();

                        else if (attachmentMenu)
                            BuyAttachment();

                        else if (weapInSlot > 0 && weapInSlot != weapID)
                            bComparing = !bComparing;
                    }
                    else
                    {
                        if (attachmentMenu)
                            BuyAttachment();
                        else if (extraWeap && isInspecting && canBuyStock)
                            BuyTheWeapon();
                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.Jump) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.Jump)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    //IVGame.ShowSubtitleMessage(isInspecting.ToString() + weapID.ToString() + weapInSlot.ToString());
                    if (extraWeap && isInspecting && weapInSlot > 0 && weapInSlot != weapID)
                        bComparing = !bComparing;
                    else if (isInspecting && weapInSlot == weapID)
                    {
                        attachmentMenu = !attachmentMenu;
                        currAttachmt = 0;
                        bInspectMsg = false;
                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavUp) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavUp)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (extraWeap && isInspecting && weapInSlot == weapID)
                        attachmentMenu = !attachmentMenu;
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavRight) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavRight)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (attachmentMenu)
                    {
                        for (int i = currAttachmt; i < numOfAttachmts; i++)
                        {
                            /*if (canBuyAttachment[currAttachmt + 1])
                            {
                                currAttachmt++;
                                break;
                            }
                            else*/
                            currAttachmt++;
                            if (currAttachmt >= numOfAttachmts)
                                currAttachmt = 0;

                            if (canBuyAttachment[currAttachmt])
                                break;
                        }
                    }
                    else if (extraWeap && isInspecting)
                    {
                        bComparing = false;
                        spawnWeaponInAir = false;

                        if (backroomSelectedWeap < backroomCount - 1)
                            backroomSelectedWeap++;
                        else
                            backroomSelectedWeap = 0;
                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavLeft) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavLeft)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (attachmentMenu)
                    {
                        for (int i = currAttachmt; numOfAttachmts > i; i--)
                        {
                            /*if ((currAttachmt) > 0 && canBuyAttachment[currAttachmt - 1])
                            {
                                currAttachmt--;
                                break;
                            }
                            else*/
                            currAttachmt--;
                            if (currAttachmt < 0)
                                currAttachmt = numOfAttachmts;

                            if (canBuyAttachment[currAttachmt])
                                break;
                        }
                    }
                    else if (extraWeap && isInspecting)
                    {
                        bComparing = false;
                        spawnWeaponInAir = false;

                        if (backroomSelectedWeap > 0)
                            backroomSelectedWeap--;
                        else
                            backroomSelectedWeap = backroomCount - 1;
                    }
                }
            }
        }
        private void ProcessSpeech()
        {
            if (enterGunShop)
                return;

            if (!DOES_CHAR_EXIST(GuardPeds[0]))
                return;

            enterGunShop = true;

            if (DOES_CHAR_EXIST(GuardPeds[0]) && !IS_CHAR_DEAD(GuardPeds[0]) && !IS_PED_IN_COMBAT(GuardPeds[0]))
            {
                //2484359393 - 
                //PLAY_AUDIO_EVENT("GS1_PURCH2");
                //PLAY_SOUND(soundID, "GS1_MA");
            }
        }
        private void SpawnDealer()
        {
            if (HasSpawned)
            {
                if (!HAS_MODEL_LOADED(GET_HASH_KEY(dealerMdl)))
                {
                    REQUEST_MODEL(GET_HASH_KEY(dealerMdl));
                    return;
                }

                if (!DOES_CHAR_EXIST(GuardPeds[0]))
                {
                    GuardPeds[0] = CreatePed_DontRequestModel(GET_HASH_KEY(dealerMdl), dealerPos, dealerHdg, (int)ePedType.PED_TYPE_CIV_MALE);
                    GIVE_WEAPON_TO_CHAR(GuardPeds[0], dealerWpn, -1, true);
                    ADD_ARMOUR_TO_CHAR(GuardPeds[0], 100);
                    SET_DONT_ACTIVATE_RAGDOLL_FROM_PLAYER_IMPACT(GuardPeds[0], true);
                    //SET_CHAR_RELATIONSHIP_GROUP(GuardPeds[i], (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                    //SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_COMPANION, (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                    SET_CHAR_RELATIONSHIP(GuardPeds[0], (uint)eRelationship.RELATIONSHIP_RESPECT, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);
                    SET_ROOM_FOR_CHAR_BY_NAME(GuardPeds[0], roomName);
                    MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(dealerMdl));
                }

                bSpawnDealer = true;
            }
        }
        private void SpawnGuards()
        {
            if (gTimer < fTimer + gSpawnDelay)
                return;

            string guardString = configFile.GetValue("MAIN", "GuardModels", "");
            guardMdl = guardString.Split(',').ToList();

            string guardWpns = configFile.GetValue("MAIN", "GuardWeapons", "");
            guardWpn = guardWpns.Split(',').Select(int.Parse).ToList();

            int i;
            for (i = 1; i < guardMdl.Count() + 1; i++)
            {
                if (!HAS_MODEL_LOADED(GET_HASH_KEY(guardMdl[i - 1])))
                {
                    REQUEST_MODEL(GET_HASH_KEY(guardMdl[i - 1]));
                    return;
                }
                if (!DOES_CHAR_EXIST(GuardPeds[i]))
                {
                    GuardPeds[i] = CreatePed_DontRequestModel(GET_HASH_KEY(guardMdl[i - 1]), guardPos, 0, (int)ePedType.PED_TYPE_CIV_MALE);
                    GIVE_WEAPON_TO_CHAR(GuardPeds[i], guardWpn[i - 1], -1, true);
                    ADD_ARMOUR_TO_CHAR(GuardPeds[i], 100);
                    SET_ROOM_FOR_CHAR_BY_NAME(GuardPeds[i], gRoomName);
                    MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(guardMdl[i - 1]));
                    //SET_ROOM_FOR_CHAR_BY_NAME(GuardPeds[i], "gunstore");
                    _TASK_COMBAT(GuardPeds[i], Main.PlayerHandle);
                    /*SET_CHAR_RELATIONSHIP_GROUP(GuardPeds[i], (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                    SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_COMPANION, (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                    SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_HATE, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);
                    */
                }
            }
            bSpawnGuards = false;
        }
        private void ProcessGuards()
        {
            if (!hasConfigsLoaded)
                return;

            bIsHostile = storeHostile[locations.IndexOf(location)];
            int i;
            if (DOES_CHAR_EXIST(GuardPeds[0]))
            {
                GET_KEY_FOR_CHAR_IN_ROOM(GuardPeds[0], out rKey);
                GET_KEY_FOR_CHAR_IN_ROOM(Main.PlayerHandle, out pKey);
                if (!bIsHostile)
                {
                    _TASK_LOOK_AT_CHAR(GuardPeds[0], Main.PlayerHandle, 0, 0);

                    if (IS_CHAR_GESTURING(GuardPeds[0]) && (IS_PLAYER_TARGETTING_CHAR(Main.PlayerIndex, GuardPeds[0]) || IS_PLAYER_FREE_AIMING_AT_CHAR(Main.PlayerIndex, GuardPeds[0])))
                    {
                        SET_BLOCKING_OF_NON_TEMPORARY_EVENTS(GuardPeds[0], true);
                        _TASK_AIM_GUN_AT_CHAR(GuardPeds[0], Main.PlayerHandle, 45000);
                    }

                    else if (!IS_PLAYER_TARGETTING_CHAR(Main.PlayerIndex, GuardPeds[0]) && !IS_PLAYER_FREE_AIMING_AT_CHAR(Main.PlayerIndex, GuardPeds[0]))
                    {
                        CLEAR_CHAR_TASKS(GuardPeds[0]);
                        SET_BLOCKING_OF_NON_TEMPORARY_EVENTS(GuardPeds[0], false);
                    }

                    /*if (rKey == pKey)
                        ProcessSpeech();*/
                }
            }

            for (i = 0; i < (1 + guardMdl.Count()); i++)
            {
                if (DOES_CHAR_EXIST(GuardPeds[i]))
                {
                    if (bIsHostile && rKey == pKey)
                    {
                        SET_CHAR_RELATIONSHIP_GROUP(GuardPeds[i], (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_COMPANION, (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_HATE, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);
                        //_TASK_COMBAT(GuardPeds[i],Main.PlayerHandle);
                        //bIsHostile = true;
                    }
                    if (!bIsHostile && !isInspecting && (IS_CHAR_DEAD(GuardPeds[i]) || IS_PED_IN_COMBAT(GuardPeds[i]) || HAS_CHAR_BEEN_DAMAGED_BY_CHAR(GuardPeds[i], Main.PlayerHandle, true) || (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0) && rKey == pKey)))
                    {
                        GET_GAME_TIMER(out fTimer);
                        GET_GAME_TIMER(out attackTimers[locations.IndexOf(location)]);
                        /*SET_CHAR_RELATIONSHIP_GROUP(GuardPeds[i], (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_COMPANION, (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(GuardPeds[i], (uint)eRelationship.RELATIONSHIP_HATE, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);
                        _TASK_COMBAT(GuardPeds[i], Main.PlayerHandle);*/
                        bSpawnGuards = true;
                        storeHostile[locations.IndexOf(location)] = true;
                    }
                }
            }

        }
        private void StopText()
        {
            if (gTimer >= fTimer + 2000 && bFailMsg)
                bFailMsg = false;
        }
        private bool InitialChecks()
        {
            if (IS_SCREEN_FADED_OUT()) return false;
            if (IS_PAUSE_MENU_ACTIVE()) return false;
            return true;
        }
        public void Main_Tick(object sender, EventArgs e)
        {
            if (enable && (currEp != 0 || FaustProg > 20 || !unlockDYHP))
            {
                PlayerPed = IVPed.FromUIntPtr(IVPlayerInfo.FindThePlayerPed());
                PlayerHandle = PlayerPed.GetHandle();
                PlayerIndex = (int)GET_PLAYER_ID();
                PlayerPos = PlayerPed.Matrix.Pos;

                if (!InitialChecks())
                    return;
                if (PlayerPed == null)
                    return;

                currEp = GET_CURRENT_EPISODE();
                if (NativeGame.IsScriptRunning("gunlockup"))
                    TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("gunlockup");
                if (NativeGame.IsScriptRunning("gunlockupct"))
                    TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("gunlockupct");

                //ObjectHelper.GrabAllObjs();

                GET_GAME_TIMER(out gTimer);

                float gProg;

                if (currEp == 0)
                    gProg = GET_FLOAT_STAT(0);
                else if (currEp == 1)
                    gProg = GET_FLOAT_STAT(133);
                else
                    gProg = GET_FLOAT_STAT(187);

                if (GameProg < gProg)
                {
                    MissionTracker();
                    GameProg = gProg;
                }
                CheckSideMissions();
                //IVGame.ShowSubtitleMessage(gProg.ToString() + " Game " + GameProg.ToString() + " fix?" + GET_FLOAT_STAT(48).ToString() + " er?" + GET_FLOAT_STAT(43).ToString() + " ass?" + GET_FLOAT_STAT(121).ToString());

                //IVGame.ShowSubtitleMessage(gProg.ToString() + locations.IndexOf(location).ToString() + " Game " + GameProg.ToString() + isWeapUnlocked.ToString());
                for (int i = 0; i < gunStores.Count(); i++)
                {
                    if (LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, locations[i].X, locations[i].Y, locations[i].Z, 50, 50, 50, false) && !hasConfigsLoaded)
                    {
                        GetGameProgress();
                        LoadColors(Settings);
                        LoadConfigs(i);
                        location = locations[i];
                        if (gTimer >= (attackTimers[i] + cooldown))
                            storeHostile[locations.IndexOf(location)] = false;
                        if (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
                            SET_STATE_OF_CLOSEST_DOOR_OF_TYPE((uint)doorHash, doorPos.X, doorPos.Y, doorPos.Z, true, 0.0f);
                        hasConfigsLoaded = true;
                        break;
                    }
                    else if (!LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 50, 50, 50, false) && hasConfigsLoaded)
                        hasConfigsLoaded = false;
                }
                SpawnGuns();
                BuyGuns();
                ProcessPurchase();
                StopText();
                AnimCheck();
                ProcessGuards();
                ProcessAttachments();
                if (goInspect)
                    ProcessInspectAnims();
                if (camActivate)
                    ProcessInspectionCam();
                if (!bSpawnDealer)
                    SpawnDealer();
                if (bSpawnGuards)
                    SpawnGuards();

                if (DID_SAVE_COMPLETE_SUCCESSFULLY() && GET_IS_DISPLAYINGSAVEMESSAGE())
                {
                    for (int i = 0; i < numOfWeaponIDs; i++)
                    {
                        if (Settings.DoesSectionExists(i.ToString()))
                            WriteBooleanToINI(Settings, i.ToString() + "Unlocked", weapUnlocks[i]);

                        /*if (weapFuncsConfig.DoesSectionExists(i.ToString()))
                        {
                            WriteBooleanToINI(attachmentsConfig, i + "HasGLAttachment", attachmentUnlocks[i]);
                            //WriteIntToINI(attachmentsConfig, i + "HasGLAttachment", 0);
                        }*/
                    }
                    Settings.Save();
                    attachmentsConfig.Save();
                }
                //ObjectLocator.Tick();
            }
        }
    }
}
