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
        private static bool unlockDYHP;
        private static bool buyAmmoEarly;
        private static bool keepAmmo;
        private static bool extraStock;
        private static int numOfWeaponIDs;
        private static int cooldown;
        private static int gSpawnDelay;

        // HUDColors
        private static int hudR;
        private static int hudG;
        private static int hudB;
        private static int hudTextR;
        private static int hudTextG;
        private static int hudTextB;

        // GameStuff
        private static uint gTimer;
        private static uint fTimer;
        private static uint currEp;
        private static Vector3 location;
        private static int weapID = 0;
        private static Vector3 wCoords;
        private static float wRad = 0;
        private static int backroomSelectedWeap = 0;
        private static float animTime;
        private static int camera;
        private static int weapInAir;
        private static int weapChecked;

        private static int weapSlot;
        private static int weapInSlot;
        private static int ammoInSlot;
        private static int maxAmmoInSlot;
        private static int soundID = -1;

        // PlayerStuff
        private static uint pMoney;
        private static int pAmmo;
        private static int maxAmmo;

        // BooleShit
        private static bool hasGameLoaded = false;
        private static bool hasConfigsLoaded = false;
        private static bool hasSpawned = false;
        private static bool isWeapUnlocked = false;
        private static bool confirmation = false;
        private static bool menuActive = false;
        private static bool goInspect = false;
        private static bool camActivate = false;
        private static bool bInspectMsg = false;
        private static bool bFailMsg = false;
        private static bool canInspect = false;
        private static bool attachmentMenu = false;
        private static bool extraWeap = false;
        private static bool isInspecting = false;
        private static bool bComparing = false;
        private static bool canBuyStock = false;
        private static bool bSpawnGuards = false;

        private static bool spawnWeaponInAir = false;

        // AttachmentShit
        private static int currAttachmt = 0;
        private static WeaponHelper.Attachment[][] wAttachments;

        // Lists, Arrays
        private static List<GunStore> gunstoreData = new List<GunStore>();
        private static List<SettingsFile> gunStores = new List<SettingsFile>();
        private static List<int> backroomWeaps = new List<int>();
        private static WeaponHelper.Weapon[] weaponData;
        private static WeaponHelper.WeaponStat[] weaponStat;
        private static int[] weaponProps;
        private static int[] guardPeds;

        private static GunStore currGunStore;

        // SettingsFiles
        private static SettingsFile wfAttachmentsConfig;
        private static SettingsFile attachmentsConfig;

        public Main()
        {
            Uninitialize += Main_Uninitialize;
            Initialized += Main_Initialized;
            IngameStartup += Main_IngameStartup;
            Tick += Main_Tick;
        }

        // HelperFunctions
        private void RequestShit()
        {
            if (!HAVE_ANIMS_LOADED("missgunlockup"))
                REQUEST_ANIMS("missgunlockup");
            if (!HAVE_ANIMS_LOADED("pickup_object"))
                REQUEST_ANIMS("pickup_object");
            if (!IS_FONT_LOADED(6))
                LOAD_TEXT_FONT(6);
        }
        private void RemoveShit()
        {
            REMOVE_ANIMS("missgunlockup");
            REMOVE_ANIMS("pickup_object");
            UNLOAD_TEXT_FONT();
        }
        private void MissionTracker(int i)
        {
            float floatProg = GET_FLOAT_STAT(weaponData[i].UnlockStat);
            int intProg = GET_INT_STAT(weaponData[i].UnlockStat);

            if (intProg >= weaponData[i].UnlockProg || floatProg >= weaponData[i].UnlockProg)
                isWeapUnlocked = true;
            else
                isWeapUnlocked = false;
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
        private void DespawnShit()
        {
            int i;
            for (i = 0; i < numOfWeaponIDs; i++)
            {
                if (!DOES_OBJECT_EXIST(weaponProps[i]))
                    continue;

                DELETE_OBJECT(ref weaponProps[i]);
            }

            for (i = 0; i < 4; i++)
            {
                if (DOES_CHAR_EXIST(guardPeds[i]))
                {
                    if (!IS_PED_IN_COMBAT(guardPeds[i]))
                        DELETE_CHAR(ref guardPeds[i]);
                    else
                        MARK_CHAR_AS_NO_LONGER_NEEDED(guardPeds[i]);
                }
            }
        }
        private void ClearShit()
        {
            if (IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GS1_GREET1_04") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON") || IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GLOCK_WH2"))
                CLEAR_HELP();
            backroomSelectedWeap = 0;
            extraWeap = false;
            confirmation = false;
            canInspect = false;
            attachmentMenu = false;
        }
        private void SpawnTheWeapon(int i)
        {
            if (IS_MODEL_IN_CDIMAGE(GET_HASH_KEY(weaponData[i].Model)))
            {
                if (!HAS_MODEL_LOADED(GET_HASH_KEY(weaponData[i].Model)))
                    REQUEST_MODEL(GET_HASH_KEY(weaponData[i].Model));

                else
                {
                    weaponProps[i] = CreateObject_DontRequestModel(GET_HASH_KEY(weaponData[i].Model), 0.0f, 0.0f, 0.0f, 0.0f);
                    SET_OBJECT_COORDINATES(weaponProps[i], weaponData[i].Position);
                    SET_OBJECT_ROTATION(weaponProps[i], weaponData[i].Rotation);
                    MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(weaponData[i].Model));
                    
                    if (currGunStore.RoomName != "None")
                        ADD_OBJECT_TO_INTERIOR_ROOM_BY_NAME(weaponProps[i], currGunStore.RoomName);

                    FREEZE_OBJECT_POSITION(weaponProps[i], true);
                }
            }
        }
        private void ReplaceText()
        {
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY2_03", "Slot");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE1_01", "x~1~");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE1_02", "Melee");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE1_03", "Handguns");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE2_01", "Shotguns");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE2_02", "SMGs");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_BYE2_03", "Assault Rifles");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY2_03", "Sniper Rifles");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_01", "Heavy");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_02", "Thrown");
            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_03", "Special");
        }

        // MainProcesses
        private void SpawnGuns()
        {
            if (!hasConfigsLoaded)
                return;

            if (LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 20, 20, 20, false) && !IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
            {
                RequestShit();
                if (!hasSpawned)
                {
                    CLEAR_AREA_OF_CARS(location.X, location.Y, location.Z, 10);
                    for (int i = 0; i < currGunStore.DoorHash.Count(); i++)
                        SET_STATE_OF_CLOSEST_DOOR_OF_TYPE((uint)currGunStore.DoorHash[i], currGunStore.DoorPosX[i], currGunStore.DoorPosY[i], currGunStore.DoorPosZ[i], false, 0.0f);
                    hasSpawned = true;
                }
                for (int i = 0; i < numOfWeaponIDs; i++)
                {
                    if (DOES_OBJECT_EXIST(weaponProps[i]))
                        continue;

                    if (weaponData[i] == null)
                        continue;

                    SpawnTheWeapon(i);
                }
            }
            else if (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
                hasSpawned = false;

            else if (!LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 20, 20, 20, false))
            {
                DespawnShit();
                hasSpawned = false;
            }
        }
        private void CheckIfChanged()
        {
            if (weapChecked != weapID)
            {
                weapChecked = weapID;
                confirmation = false;
            }
        }
        private void SpawnBackroomWeapon()
        {
            if (!extraWeap || !isInspecting)
                return;

            if (spawnWeaponInAir)
                return;

            if (DOES_OBJECT_EXIST(weapInAir))
                DELETE_OBJECT(ref weapInAir);

            if (IS_MODEL_IN_CDIMAGE(GET_HASH_KEY(weaponData[weapID].Model)))
            {
                REQUEST_MODEL(GET_HASH_KEY(weaponData[weapID].Model));
                if (!HAS_MODEL_LOADED(GET_HASH_KEY(weaponData[weapID].Model)))
                    return;

                weapInAir = CreateObject_DontRequestModel(GET_HASH_KEY(weaponData[weapID].Model), currGunStore.BackroomPos.X, currGunStore.BackroomPos.Y, currGunStore.BackroomPos.Z, currGunStore.BackroomHdng);

                GET_OFFSET_FROM_OBJECT_IN_WORLD_COORDS(weapInAir, weaponData[weapID].BackroomOffset, out Vector3 objOff);
                SET_OBJECT_COORDINATES(weapInAir, objOff);
                MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(weaponData[weapID].Model));

                if (currGunStore.RoomName != "None")
                    ADD_OBJECT_TO_INTERIOR_ROOM_BY_NAME(weapInAir, currGunStore.RoomName);

                FREEZE_OBJECT_POSITION(weapInAir, true);
                spawnWeaponInAir = true;
            }
        }
        private void BuyGuns()
        {
            if (!hasSpawned)
                return;

            if (!currGunStore.Hostile)
            {
                for (int i = 0; i < numOfWeaponIDs; i++)
                {
                    if (!DOES_OBJECT_EXIST(weaponProps[i]))
                        continue;

                    if (LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, weaponData[i].BuyPos.X, weaponData[i].BuyPos.Y, weaponData[i].BuyPos.Z, weaponData[i].BuyRad, weaponData[i].BuyRad, weaponData[i].BuyRad, showBuyMarkers ? true : false))
                    {
                        weapID = i;
                        CheckIfChanged();
                        MissionTracker(i);
                        weaponStat[i].Pellets = (int)IVWeaponInfo.GetWeaponInfo((uint)i).AimingPellets;
                        wCoords = weaponData[i].BuyPos;
                        wRad = weaponData[i].BuyRad;
                        if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, i)))
                        {
                            if (!confirmation && !bInspectMsg && !bFailMsg && !menuActive)
                            {
                                if (!HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, i) && weapID != 48)
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weaponData[i].Name + " $" + weaponData[i].WeaponPrice.ToString());
                                else if (weapID == 48)
                                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponData[i].Name + " $" + weaponData[i].WeaponPrice.ToString());
                                else
                                {
                                    if (IVWeaponInfo.GetWeaponInfo((uint)i).ClipSize <= 0)
                                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weaponData[i].Name + " $" + weaponData[i].AmmoPrice.ToString());
                                    else if(IVWeaponInfo.GetWeaponInfo((uint)i).WeaponSlot <= 1 || IVWeaponInfo.GetWeaponInfo((uint)i).WeaponSlot > 7)
                                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weaponData[i].ClipAmmo.ToString() + "x " + weaponData[i].Name + "s $" + weaponData[i].AmmoPrice.ToString());
                                    else
                                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~g~" + weaponData[i].ClipAmmo.ToString() + "x " + weaponData[i].Name + " Ammo $" + weaponData[i].AmmoPrice.ToString());
                                }

                                PRINT_HELP_FOREVER("GS1_GREET1_04");
                            }
                            canInspect = true;
                        }
                        else if (!IS_THIS_HELP_MESSAGE_BEING_DISPLAYED("GL_SOON"))
                            PRINT_HELP_FOREVER("GL_SOON");
                        break;
                    }
                }
                if (extraStock && LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, currGunStore.BackroomPos.X, currGunStore.BackroomPos.Y, currGunStore.BackroomPos.Z, 1, 1, 1, extraStock ? true : false))
                {
                    if (backroomWeaps.Count > 0)
                    {
                        weapID = backroomWeaps[backroomSelectedWeap];
                        CheckIfChanged();
                        if (!menuActive)
                            isWeapUnlocked = true;
                        else
                            MissionTracker(weapID);

                        weaponStat[weapID].Pellets = (int)IVWeaponInfo.GetWeaponInfo((uint)weapID).AimingPellets;
                        if (!confirmation && !bInspectMsg && !bFailMsg)
                        {
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~s~Press ~INPUT_PICKUP~ to browse extra weapons.");
                            PRINT_HELP_FOREVER("GS1_GREET1_04");
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
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_04", "~r~There are no extra weapons in stock.");
                        PRINT_HELP_FOREVER("GS1_GREET1_04");
                    }
                }
                else if (!LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, wRad, wRad, wRad, false) && !LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, currGunStore.BackroomPos.X, currGunStore.BackroomPos.Y, currGunStore.BackroomPos.Z, 1, 1, 1, false))
                    ClearShit();
            }
            else
                ClearShit();
        }
        private void ProcessInspectionCam()
        {
            if (!camActivate)
                return;

            if (!extraWeap)
            {
                if (animTime > 0.5)
                {
                    if (isInspecting)
                    {
                        if (DOES_OBJECT_EXIST(weaponProps[weapID]))
                            SET_OBJECT_VISIBLE(weaponProps[weapID], false);

                        CreateCamera(camera, true, 0.9f, -0.3f, 1.1f, weaponData[weapID].Position, 60);

                        if (weapInSlot != weapID)
                        {
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapInSlot);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, weaponData[weapID].ClipAmmo, true);
                        }

                        SET_CURRENT_CHAR_WEAPON(Main.PlayerHandle, weapID, true);
                        menuActive = true;
                    }

                    else
                    {
                        DestroyCamera();
                        SET_PLAYER_CONTROL(Main.PlayerIndex, true);

                        if (weapInSlot > 0)
                        {
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapID);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapInSlot, ammoInSlot, false);
                        }
                        else
                            REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapID);

                        for (int i = 0; i < numOfWeaponIDs; i++)
                        {
                            if (!DOES_OBJECT_EXIST(weaponProps[i]))
                                continue;

                            SET_OBJECT_VISIBLE(weaponProps[i], true);
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
                    CreateCamera(camera, true, 0.0f, 0.5f, 0.2f, pOff, 60);
                    spawnWeaponInAir = false;
                    menuActive = true;
                    if (DOES_OBJECT_EXIST(weapInAir))
                        DELETE_OBJECT(ref weapInAir);
                }
                else
                {
                    DestroyCamera();
                    SET_PLAYER_CONTROL(Main.PlayerIndex, true);
                    spawnWeaponInAir = false;
                    menuActive = false;
                    if (DOES_OBJECT_EXIST(weapInAir))
                        DELETE_OBJECT(ref weapInAir);
                }
                camActivate = false;
            }
        }
        private void ProcessInspectAnims()
        {
            if (!goInspect)
                return;

            if (!isInspecting)
            {
                if (LOCATE_CHAR_ON_FOOT_3D(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z, 1.5f, 1.5f, 1.5f, false))
                {
                    CLEAR_CHAR_TASKS_IMMEDIATELY(Main.PlayerHandle);
                    SET_CHAR_COORDINATES(Main.PlayerHandle, wCoords.X, wCoords.Y, wCoords.Z - 1.0f);
                    GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                    GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);

                    if (!extraWeap)
                    {
                        SET_CHAR_HEADING(Main.PlayerHandle, weaponData[weapID].BuyHdng);
                        switch (weaponData[weapID].AnimStart)
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
                    }
                    else
                        SET_CHAR_HEADING(Main.PlayerHandle, currGunStore.BackroomHdng);
                    camActivate = true;
                    goInspect = false;
                    isInspecting = true;
                }
            }

            else
            {
                if (!extraWeap)
                {
                    switch (weaponData[weapID].AnimEnd)
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
                }
                camActivate = true;
                goInspect = false;
                isInspecting = false;
                bComparing = false;
                bInspectMsg = false;
                CLEAR_CHAR_TASKS(Main.PlayerHandle);
            }
        }
        private void HelpDisplay()
        {
            if (!bInspectMsg || (extraWeap && !confirmation && !bFailMsg) || (attachmentMenu && !confirmation && !bFailMsg))
            {
                if (!extraWeap)
                {
                    if (!attachmentMenu)
                    {
                        if (weapInSlot <= 0 || weapInSlot == weapID)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Press ~INPUT_PICKUP~ to stop inspecting.");
                        else if (weapInSlot > 0)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Press ~INPUT_PICKUP~ to stop inspecting. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to compare with current weapon in slot.");
                        for (int i = 0; i < numOfAttachmts; i++)
                        {
                            if (wAttachments[weapID][i].Available && weapID == weapInSlot)
                            {
                                IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Press ~INPUT_PICKUP~ to stop inspecting. ~n~~s~Press ~INPUT_JUMP~ to browse attachments.");
                                break;
                            }
                        }
                    }
                    else
                        ProcessAttachmentsHelp();
                }
                else
                {
                    GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                    GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);
                    if (!attachmentMenu)
                    {
                        if (weapInSlot <= 0)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponData[weapID].Name + " $" + weaponData[weapID].WeaponPrice.ToString());
                        else if (weapInSlot == weapID)
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponData[weapID].ClipAmmo.ToString() + "x " + weaponData[weapID].Name + " Ammo $" + weaponData[weapID].AmmoPrice.ToString() + "~n~~s~Currently have ~g~" + ammoInSlot.ToString() + "x ~s~ammo.");
                        else
                            IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse weapons. ~n~~s~Press ~INPUT_PICKUP~ to exit. ~n~~s~Press ~INPUT_JUMP~ to compare with current weapon in the same slot. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~" + weaponData[weapID].Name + " $" + weaponData[weapID].WeaponPrice.ToString());
                    }
                    else
                        ProcessAttachmentsHelp();
                }
                if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID)))
                    PRINT_HELP_FOREVER("GS1_ASST1_02");

                bInspectMsg = true;
            }
        }
        private void AnimCheck()
        {
            if (!canInspect)
                return;

            if (isInspecting)
            {
                switch (weaponData[weapID].AnimStart)
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
                switch (weaponData[weapID].AnimEnd)
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
        private void DisplayNumericalStats(WeaponHelper.WeaponStat weap, float offset)
        {
            if (weap.StatA >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset, "NUMBER", weap.StatA);

                if (weapSlot == 3 && weap.Pellets != 1)
                {
                    USE_PREVIOUS_FONT_SETTINGS();
                    if (weap.Pellets > 1)
                        DISPLAY_TEXT_WITH_NUMBER(0.958f, offset, "GS1_BYE1_01", weap.Pellets + 2);
                    else
                        DISPLAY_TEXT_WITH_NUMBER(0.958f, offset, "GS1_BYE1_01", 18);
                }
            }

            if (weap.StatB >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.04f, "NUMBER", weap.StatB);
            }

            if (weap.StatC >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.08f, "NUMBER", weap.StatC);
            }

            if (weap.StatD >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.12f, "NUMBER", weap.StatD);
            }

            if (weap.StatE >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.16f, "NUMBER", weap.StatE);
            }

            if (weap.StatF >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.20f, "NUMBER", weap.StatF);
            }

            if (weap.StatG >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.24f, "NUMBER", weap.StatG);
            }

            if (weap.StatH >= 0)
            {
                USE_PREVIOUS_FONT_SETTINGS();
                DISPLAY_TEXT_WITH_NUMBER(0.94f, offset + 0.28f, "NUMBER", weap.StatH);
            }
        }
        private void DrawBars(WeaponHelper.WeaponStat weap, float offset)
        {
            //Weapon Bars
            float damageSize = weap.BarA / 1000.0f;
            float accuracySize = weap.BarB / 1000.0f;
            float rangeSize = weap.BarC / 1000.0f;
            float RoFSize = weap.BarD / 1000.0f;
            float magSize = weap.BarE / 1000.0f;
            float ammoSize = weap.BarF / 1000.0f;
            float aimSize = weap.BarG / 1000.0f;
            float forceSize = weap.BarH / 1000.0f;

            float damagePos = 0.882f - (0.1f - damageSize) / 2;
            float accuracyPos = 0.882f - (0.1f - accuracySize) / 2;
            float rangePos = 0.882f - (0.1f - rangeSize) / 2;
            float RoFPos = 0.882f - (0.1f - RoFSize) / 2;
            float magPos = 0.882f - (0.1f - magSize) / 2;
            float ammoPos = 0.882f - (0.1f - ammoSize) / 2;
            float aimPos = 0.882f - (0.1f - aimSize) / 2;
            float forcePos = 0.882f - (0.1f - forceSize) / 2;

            if (weap.BarA >= 0)
            {
                DRAW_RECT(0.882f, offset, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(damagePos, offset, damageSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarB >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.04f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(accuracyPos, offset + 0.04f, accuracySize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarC >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.08f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(rangePos, offset + 0.08f, rangeSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarD >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.12f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(RoFPos, offset + 0.12f, RoFSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarE >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.16f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(magPos, offset + 0.16f, magSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarF >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.20f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(ammoPos, offset + 0.20f, ammoSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarG >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.24f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(aimPos, offset + 0.24f, aimSize, 0.015f, 255, 255, 255, 255);
            }
            if (weap.BarH >= 0)
            {
                DRAW_RECT(0.882f, offset + 0.28f, 0.1f, 0.015f, 50, 50, 50, 255);
                DRAW_RECT(forcePos, offset + 0.28f, forceSize, 0.015f, 255, 255, 255, 255);
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
                IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_03", weaponData[weapID].Name);
                DISPLAY_TEXT(0.85f, 0.07f, "GS1_ASST1_03");

                //Weapon Stats Text
                if (isWeapUnlocked || (buyAmmoEarly && HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID)))
                {
                    SET_TEXT_SCALE(0.3f, 0.3f);
                    SET_TEXT_FONT(6);
                    SET_TEXT_COLOUR((uint)hudTextR, (uint)hudTextG, (uint)hudTextB, 255);
                    SET_TEXT_WRAP(0.725f, 0.975f);
                    SET_TEXT_DROPSHADOW(false, 0, 0, 0, 0);

                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST2_01", weaponStat[weapID].StatTextA);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST2_02", weaponStat[weapID].StatTextB);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST2_03", weaponStat[weapID].StatTextC);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY1_01", weaponStat[weapID].StatTextD);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY1_02", weaponStat[weapID].StatTextE);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY1_03", weaponStat[weapID].StatTextF);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY2_01", weaponStat[weapID].StatTextG);
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_AWAY2_02", weaponStat[weapID].StatTextH);

                    DISPLAY_TEXT(0.725f, 0.19f, "GS1_AWAY2_03");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.23f, "GS1_ASST2_01");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.27f, "GS1_ASST2_02");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.31f, "GS1_ASST2_03");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.35f, "GS1_AWAY1_01");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.39f, "GS1_AWAY1_02");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.43f, "GS1_AWAY1_03");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.47f, "GS1_AWAY2_01");
                    USE_PREVIOUS_FONT_SETTINGS();
                    DISPLAY_TEXT(0.725f, 0.51f, "GS1_AWAY2_02");

                    //Weapon Slots & Numerical Info
                    DisplayNumericalStats(weaponStat[weapID], 0.23f);

                    USE_PREVIOUS_FONT_SETTINGS();
                    SET_TEXT_COLOUR(255, 255, 255, 255);
                    if (weapSlot == 1)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_BYE1_02");
                    else if (weapSlot == 2)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_BYE1_03");
                    else if (weapSlot == 3)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_BYE2_01");
                    else if (weapSlot == 4)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_BYE2_02");
                    else if (weapSlot == 5)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_BYE2_03");
                    else if (weapSlot == 6)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_MONKEY2_03");
                    else if (weapSlot == 7)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_GREET1_01");
                    else if (weapSlot == 8)
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_GREET1_02");
                    else
                        DISPLAY_TEXT(0.83f, 0.19f, "GS1_GREET1_03");

                    //Weapon Bars
                    DrawBars(weaponStat[weapID], 0.24f);

                    //Weapon Messages
                    if (!bComparing)
                    {
                        USE_PREVIOUS_FONT_SETTINGS();
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET1_05", weaponStat[weapID].Message);
                        DISPLAY_TEXT(0.725f, 0.55f, "GS1_GREET1_05");
                    }
                    //Comparing weapons
                    else
                    {
                        SET_TEXT_SCALE(0.5f, 0.5f);
                        SET_TEXT_FONT(6);
                        SET_TEXT_COLOUR((uint)hudR, (uint)hudG, (uint)hudB, 255);
                        SET_TEXT_CENTRE(true);

                        //Weapon Names
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET2_01", weaponData[weapInSlot].Name);
                        DISPLAY_TEXT(0.85f, 0.57f, "GS1_GREET2_01");

                        //Weapon Stats Text
                        SET_TEXT_SCALE(0.3f, 0.3f);
                        SET_TEXT_FONT(6);
                        SET_TEXT_COLOUR((uint)hudTextR, (uint)hudTextG, (uint)hudTextB, 255);
                        SET_TEXT_WRAP(0.725f, 0.975f);
                        SET_TEXT_DROPSHADOW(false, 0, 0, 0, 0);

                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET2_02", weaponStat[weapInSlot].StatTextA);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_GREET2_03", weaponStat[weapInSlot].StatTextB);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY1_01", weaponStat[weapInSlot].StatTextC);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY1_02", weaponStat[weapInSlot].StatTextD);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY1_03", weaponStat[weapInSlot].StatTextE);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY1_04", weaponStat[weapInSlot].StatTextF);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY1_05", weaponStat[weapInSlot].StatTextG);
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY2_01", weaponStat[weapInSlot].StatTextH);

                        DISPLAY_TEXT(0.725f, 0.64f, "GS1_GREET2_02");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.68f, "GS1_GREET2_03");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.72f, "GS1_MONKEY1_01");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.76f, "GS1_MONKEY1_02");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.80f, "GS1_MONKEY1_03");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.84f, "GS1_MONKEY1_04");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.88f, "GS1_MONKEY1_05");
                        USE_PREVIOUS_FONT_SETTINGS();
                        DISPLAY_TEXT(0.725f, 0.92f, "GS1_MONKEY2_01");

                        //Weapon Slots & Numerical Info
                        DisplayNumericalStats(weaponStat[weapInSlot], 0.64f);

                        //Weapon Bars
                        DrawBars(weaponStat[weapInSlot], 0.65f);
                    }
                }
                HelpDisplay();

                if (animTime > 0.5)
                {
                    if (weaponData[weapID].AnimLoop == 0 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "over_shoulder"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "over_shoulder", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 1 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "rifle_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rifle_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 2 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "uzi_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "uzi_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 3 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "rpg_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "rpg_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 4 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "pistol_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "pistol_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 5 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "grenade_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "grenade_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                    else if (weaponData[weapID].AnimLoop == 6 && !IS_CHAR_PLAYING_ANIM(Main.PlayerHandle, "missgunlockup", "shotgun_loop"))
                        _TASK_PLAY_ANIM_NON_INTERRUPTABLE(Main.PlayerHandle, "shotgun_loop", "missgunlockup", 4.0f, 1, 0, 0, 0, 0);
                }
            }
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
        private void FullAmmo()
        {
            GET_MAX_AMMO_IN_CLIP(Main.PlayerHandle, weapID, out int wClipAmmo);
            if (wClipAmmo > 0)
                IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY2_02", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~n~~r~You can't carry more of that type of ammo.");
            else
                IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY2_02", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~n~~r~You already own a " + weaponData[weapID].Name + ".");

            PRINT_HELP("GS1_MONKEY2_02");
            GET_GAME_TIMER(out fTimer);
            bFailMsg = true;
        }
        private void BuyTheWeapon()
        {
            if (!extraWeap)
            {
                GET_WEAPONTYPE_SLOT(weapID, out weapSlot);
                GET_CHAR_WEAPON_IN_SLOT(Main.PlayerHandle, weapSlot, out weapInSlot, out ammoInSlot, out maxAmmoInSlot);
            }
            GET_CHAR_ARMOUR(Main.PlayerHandle, out uint pArmr);
            GET_MAX_AMMO_IN_CLIP(Main.PlayerHandle, weapID, out int wClipAmmo);

            if (pAmmo < maxAmmo)
            {
                if (HAS_CHAR_GOT_WEAPON(Main.PlayerHandle, weapID) && pMoney >= weaponData[weapID].AmmoPrice)
                {
                    if (weapID != 48)
                    {
                        if (wClipAmmo > 0)
                        {
                            PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                            ADD_SCORE(Main.PlayerIndex, -weaponData[weapID].AmmoPrice);
                            if (DOES_CHAR_EXIST(guardPeds[0]))
                                SAY_AMBIENT_SPEECH(guardPeds[0], "THANKS", false, true, 0);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, weaponData[weapID].ClipAmmo, true);
                        }
                        else
                            FullAmmo();
                    }
                    else if (pArmr < 100)
                    {
                        PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                        ADD_SCORE(Main.PlayerIndex, -weaponData[weapID].AmmoPrice);
                        if (DOES_CHAR_EXIST(guardPeds[0]))
                            SAY_AMBIENT_SPEECH(guardPeds[0], "THANKS", false, true, 0);
                        ADD_ARMOUR_TO_CHAR(Main.PlayerHandle, 100);
                    }
                }
                else if (pMoney >= weaponData[weapID].WeaponPrice)
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
                            PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                            ADD_SCORE(Main.PlayerIndex, -weaponData[weapID].WeaponPrice);
                            //PLAY_AUDIO_EVENT_FROM_PED("GS1_BUY", GuardPeds[0]);
                            //int b = GET_HASH_KEY("GUN_NUT");
                            //NEW_SCRIPTED_CONVERSATION();
                            //ADD_NEW_FRONTEND_CONVERSATION_SPEAKER(GuardPeds[0], b);
                            //ADD_NEW_CONVERSATION_SPEAKER(0, GuardPeds[0], out b);
                            //ADD_LINE_TO_CONVERSATION(0, GET_HASH_KEY("GS1_MA"), GET_HASH_KEY("GS1_PURCH2"), 0, 0);
                            //START_SCRIPT_CONVERSATION(true, true);
                            //IVGame.ShowSubtitleMessage(b.ToString() + "   " + GET_HASH_KEY("GS1_MA") + "   " + unchecked((int)2484359393));

                            if (DOES_CHAR_EXIST(guardPeds[0]))
                                SAY_AMBIENT_SPEECH(guardPeds[0], "THANKS", false, true, 0);
                            if (!keepAmmo)
                                REMOVE_WEAPON_FROM_CHAR(Main.PlayerHandle, weapInSlot);
                            GIVE_WEAPON_TO_CHAR(Main.PlayerHandle, weapID, weaponData[weapID].ClipAmmo, true);
                        }
                        else if (pArmr < 100)
                        {
                            PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                            ADD_SCORE(Main.PlayerIndex, -weaponData[weapID].WeaponPrice);
                            if (DOES_CHAR_EXIST(guardPeds[0]))
                                SAY_AMBIENT_SPEECH(guardPeds[0], "THANKS", false, true, 0);
                            ADD_ARMOUR_TO_CHAR(Main.PlayerHandle, 100);
                        }
                    }
                }
                else
                {
                    IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_01", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~s~Press ~INPUT_PICKUP~ to examine. ~n~~r~You don't have enough cash for that weapon.");
                    PRINT_HELP("GS1_ASST1_01");
                    GET_GAME_TIMER(out fTimer);
                    bFailMsg = true;
                }
            }
            else
                FullAmmo();
        }
        private void ProcessPurchase()
        {
            if (!hasSpawned)
                return;

            if (!canInspect)
                return;

            STORE_SCORE(Main.PlayerIndex, out pMoney);
            GET_AMMO_IN_CHAR_WEAPON(Main.PlayerHandle, weapID, out pAmmo);
            GET_MAX_AMMO(Main.PlayerHandle, weapID, out maxAmmo);

            //Inspect anim loop and stats display
            if (menuActive)
                ProcessStatDisplay();

            if (!bFailMsg && IVPhoneInfo.ThePhoneInfo.State <= 1000)
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
                    else if (weapID != 48)
                    {
                        backroomSelectedWeap = 0;
                        wCoords = currGunStore.BackroomPos;

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
                    if (extraWeap && isInspecting && weapInSlot > 0 && weapInSlot != weapID)
                        bComparing = !bComparing;
                    else if (isInspecting && weapInSlot == weapID)
                    {
                        attachmentMenu = !attachmentMenu;
                        currAttachmt = 0;
                        bInspectMsg = false;

                        if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                            LoadAttachmentData(weapID, currAttachmt);

                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavUp) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavUp)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (extraWeap && isInspecting && weapInSlot == weapID)
                    {
                        attachmentMenu = !attachmentMenu;
                        currAttachmt = 0;

                        if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                            LoadAttachmentData(weapID, currAttachmt);

                    }
                }

                else if ((IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavRight) || IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavRight)) && !IS_CONTROL_JUST_PRESSED(0, (int)GameKey.NavEnter) && !IS_CONTROL_JUST_PRESSED(2, (int)GameKey.NavEnter))
                {
                    if (attachmentMenu)
                    {
                        for (int i = currAttachmt; i < numOfAttachmts; i++)
                        {
                            currAttachmt++;
                            if (currAttachmt >= numOfAttachmts)
                                currAttachmt = 0;

                            if (wAttachments[weapID][currAttachmt].Available)
                                break;
                        }
                    }
                    else if (extraWeap && isInspecting)
                    {
                        confirmation = false;
                        bComparing = false;
                        spawnWeaponInAir = false;

                        if (backroomSelectedWeap < backroomWeaps.Count - 1)
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

                            if (wAttachments[weapID][currAttachmt].Available)
                                break;
                        }
                    }
                    else if (extraWeap && isInspecting)
                    {
                        confirmation = false;
                        bComparing = false;
                        spawnWeaponInAir = false;

                        if (backroomSelectedWeap > 0)
                            backroomSelectedWeap--;
                        else
                            backroomSelectedWeap = backroomWeaps.Count - 1;
                    }
                }
            }
        }
        private void StopText()
        {
            if (!bFailMsg)
                return;

            if (gTimer >= fTimer + 2000)
                bFailMsg = false;
        }

        // GuardShit
        private void SpawnDealer()
        {
            if (hasSpawned)
            {
                if (IS_MODEL_IN_CDIMAGE(GET_HASH_KEY(currGunStore.DealerModel)) && IS_THIS_MODEL_A_PED((uint)GET_HASH_KEY(currGunStore.DealerModel)))
                {
                    if (!DOES_CHAR_EXIST(guardPeds[0]))
                    {
                        if (!HAS_MODEL_LOADED(GET_HASH_KEY(currGunStore.DealerModel)))
                        {
                            REQUEST_MODEL(GET_HASH_KEY(currGunStore.DealerModel));
                            return;
                        }

                        guardPeds[0] = CreatePed_DontRequestModel(GET_HASH_KEY(currGunStore.DealerModel), currGunStore.DealerPos, currGunStore.DealerHdng, (int)ePedType.PED_TYPE_CIV_MALE);
                        GIVE_WEAPON_TO_CHAR(guardPeds[0], currGunStore.DealerWpn, -1, true);
                        ADD_ARMOUR_TO_CHAR(guardPeds[0], 100);
                        SET_DONT_ACTIVATE_RAGDOLL_FROM_PLAYER_IMPACT(guardPeds[0], true);
                        SET_CHAR_RELATIONSHIP(guardPeds[0], (uint)eRelationship.RELATIONSHIP_RESPECT, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);

                        if (currGunStore.RoomName != "None")
                            SET_ROOM_FOR_CHAR_BY_NAME(guardPeds[0], currGunStore.RoomName);
                        MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(currGunStore.DealerModel));
                    }
                }
                else
                {
                    IVGame.Console.PrintError(currGunStore.DealerModel + " is not a valid ped model. Gun nut model will be used instead.");
                    currGunStore.DealerModel = "M_M_GUNNUT_01";
                }
            }
        }
        private void SpawnGuards()
        {
            if (!bSpawnGuards)
                return;

            if (gTimer < fTimer + gSpawnDelay)
                return;

            int i;
            bool notSpawned = false;
            for (i = 1; i < 4; i++)
            {
                if (IS_MODEL_IN_CDIMAGE(GET_HASH_KEY(currGunStore.GuardModels[i - 1])) && IS_THIS_MODEL_A_PED((uint)GET_HASH_KEY(currGunStore.GuardModels[i - 1])))
                {
                    if (!HAS_MODEL_LOADED(GET_HASH_KEY(currGunStore.GuardModels[i - 1])))
                    {
                        REQUEST_MODEL(GET_HASH_KEY(currGunStore.GuardModels[i - 1]));
                        notSpawned = true;
                    }
                    else if (!DOES_CHAR_EXIST(guardPeds[i]))
                    {
                        guardPeds[i] = CreatePed_DontRequestModel(GET_HASH_KEY(currGunStore.GuardModels[i - 1]), currGunStore.GuardPos, 0, (int)ePedType.PED_TYPE_CIV_MALE);
                        GIVE_WEAPON_TO_CHAR(guardPeds[i], currGunStore.GuardWpns[i - 1], -1, true);
                        ADD_ARMOUR_TO_CHAR(guardPeds[i], 100);

                        if (currGunStore.GuardRoom != "None")
                            SET_ROOM_FOR_CHAR_BY_NAME(guardPeds[i], currGunStore.GuardRoom);
                        MARK_MODEL_AS_NO_LONGER_NEEDED(GET_HASH_KEY(currGunStore.GuardModels[i - 1]));
                        _TASK_COMBAT(guardPeds[i], Main.PlayerHandle);
                        notSpawned = true;
                    }
                }
                else
                {
                    IVGame.Console.PrintError(currGunStore.GuardModels[i - 1] + " is not a valid ped model. Gun nut model will be used instead.");
                    currGunStore.GuardModels[i - 1] = "M_M_GUNNUT_01";
                }
            }
            if (!notSpawned)
            bSpawnGuards = false;
        }
        private void ProcessGuards()
        {
            if (!hasConfigsLoaded)
                return;

            int i;
            uint rKey = 0, pKey = 0;

            if (DOES_CHAR_EXIST(guardPeds[0]))
            {
                GET_KEY_FOR_CHAR_IN_ROOM(guardPeds[0], out rKey);
                GET_KEY_FOR_CHAR_IN_ROOM(Main.PlayerHandle, out pKey);
                if (!currGunStore.Hostile)
                {
                    _TASK_LOOK_AT_CHAR(guardPeds[0], Main.PlayerHandle, 0, 0);

                    if (IS_CHAR_GESTURING(guardPeds[0]) && (IS_PLAYER_TARGETTING_CHAR(Main.PlayerIndex, guardPeds[0]) || IS_PLAYER_FREE_AIMING_AT_CHAR(Main.PlayerIndex, guardPeds[0])))
                    {
                        SET_BLOCKING_OF_NON_TEMPORARY_EVENTS(guardPeds[0], true);
                        _TASK_AIM_GUN_AT_CHAR(guardPeds[0], Main.PlayerHandle, 45000);
                    }

                    else if (!IS_PLAYER_TARGETTING_CHAR(Main.PlayerIndex, guardPeds[0]) && !IS_PLAYER_FREE_AIMING_AT_CHAR(Main.PlayerIndex, guardPeds[0]))
                    {
                        CLEAR_CHAR_TASKS(guardPeds[0]);
                        SET_BLOCKING_OF_NON_TEMPORARY_EVENTS(guardPeds[0], false);
                    }

                    /*if (rKey == pKey)
                        ProcessSpeech();*/
                }
            }

            for (i = 0; i < 4; i++)
            {
                if (DOES_CHAR_EXIST(guardPeds[i]))
                {
                    if (currGunStore.Hostile && rKey == pKey)
                    {
                        SET_CHAR_RELATIONSHIP_GROUP(guardPeds[i], (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(guardPeds[i], (uint)eRelationship.RELATIONSHIP_COMPANION, (int)eRelationshipGroup.RELATIONSHIP_GROUP_MISSION_2);
                        SET_CHAR_RELATIONSHIP(guardPeds[i], (uint)eRelationship.RELATIONSHIP_HATE, (int)eRelationshipGroup.RELATIONSHIP_GROUP_PLAYER);
                    }
                    if (!currGunStore.Hostile && !isInspecting && (IS_CHAR_DEAD(guardPeds[i]) || IS_PED_IN_COMBAT(guardPeds[i]) || HAS_CHAR_BEEN_DAMAGED_BY_CHAR(guardPeds[i], Main.PlayerHandle, true) || (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0) && rKey == pKey)))
                    {
                        GET_GAME_TIMER(out fTimer);
                        GET_GAME_TIMER(out uint currTimer);

                        currGunStore.AttackTimer = currTimer;
                        bSpawnGuards = true;
                        currGunStore.Hostile = true;
                    }
                }
            }

        }

        // Blips
        private void ClearBlips()
        {
            for (int i = 0; i < gunStores.Count(); i++)
            {
                if (DOES_BLIP_EXIST(gunstoreData[i].Blip))
                    REMOVE_BLIP(gunstoreData[i].Blip);
            }
        }
        private void AddOrRemoveBlip(int stat, int prog, int storeID)
        {
            if (GET_FLOAT_STAT(stat) > prog || currEp > 0)
            {
                if (!DOES_BLIP_EXIST(gunstoreData[storeID].Blip))
                {
                    ADD_BLIP_FOR_COORD(gunstoreData[storeID].Location.X, gunstoreData[storeID].Location.Y, gunstoreData[storeID].Location.Z, out int pBlip);
                    gunstoreData[storeID].Blip = pBlip;
                    CHANGE_BLIP_SPRITE(gunstoreData[storeID].Blip, (int)BlipIcon.Building_WeaponShop);
                    CHANGE_BLIP_DISPLAY(gunstoreData[storeID].Blip, (uint)eBlipDisplay.BLIP_DISPLAY_ARROW_AND_MAP);
                    SET_BLIP_AS_SHORT_RANGE(gunstoreData[storeID].Blip, true);
                }
            }
            else if (DOES_BLIP_EXIST(gunstoreData[storeID].Blip))
                REMOVE_BLIP(gunstoreData[storeID].Blip);
        }
        private void ProcessBlips(int gunstore)
        {
            switch (gunstoreData[gunstore].IconDisplay)
            {
                // DYHP
                case 0:
                    AddOrRemoveBlip(8, 25, gunstore);
                    break;

                // BlowYourCover
                case 1:
                    AddOrRemoveBlip(10, 40, gunstore);
                    break;

                // TLC
                case 2:
                    AddOrRemoveBlip(22, 90, gunstore);
                    break;
            }
        }

        // AttachmentShit
        private static string GetAttachmentName(int attachID)
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
        private void ProcessAttachmentsHelp()
        {
            switch (currAttachmt)
            {
                case 0:
                    if (!wAttachments[weapID][0].Owned)
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Grenade Launcher attachment $" + wAttachments[weapID][0].Price.ToString());
                    else
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~g~$" + wAttachments[weapID][0].AmmoPrice.ToString() + "~n~~s~Currently have ~g~" + wAttachments[weapID][0].Ammo.ToString() + "x ~s~ammo.");
                    break;
                case 1:
                    if (!wAttachments[weapID][1].Owned)
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy. ~n~~g~Scope attachment $" + wAttachments[weapID][1].Price.ToString());
                    else
                        IVText.TheIVText.ReplaceTextOfTextLabel("GS1_ASST1_02", "~s~Use ~PAD_LEFT~ and ~PAD_RIGHT~ to browse attachments. ~n~~s~Press ~INPUT_PICKUP~ to cancel. ~n~~s~Already own ~n~~g~Scope attachment for this weapon.");

                    break;
            }
        }
        private void LoadAttachmentData(int w, int a)
        {
            wfAttachmentsConfig.Load();
            wAttachments[w][a].Owned = wfAttachmentsConfig.GetBoolean(IVGenericGameStorage.ValidSaveName, (w.ToString() + "Has" + GetAttachmentName(a) + "Attachment"), false);
            wAttachments[w][a].Ammo = wfAttachmentsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, w.ToString() + GetAttachmentName(a) + "Ammo", 0);
        }
        private void BuyAttachment()
        {
            if (!wAttachments[weapID][currAttachmt].Owned)
            {
                if (pMoney >= wAttachments[weapID][currAttachmt].Price)
                {
                    if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                    {
                        LoadAttachmentData(weapID, currAttachmt);

                        PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                        wAttachments[weapID][currAttachmt].Owned = true;
                        wAttachments[weapID][currAttachmt].Ammo = 1;
                        WriteBooleanToINI(wfAttachmentsConfig, (weapID.ToString() + "Has" + GetAttachmentName(currAttachmt) + "Attachment"), true);
                        WriteIntToINI(wfAttachmentsConfig, (weapID.ToString() + GetAttachmentName(currAttachmt) + "Ammo"), 1);
                        wfAttachmentsConfig.Save();
                        wfAttachmentsConfig.Load();

                        ADD_SCORE(Main.PlayerIndex, -wAttachments[weapID][currAttachmt].Price);
                    }
                }
            }
            else if (currAttachmt == 0 && pMoney >= wAttachments[weapID][currAttachmt].AmmoPrice && wAttachments[weapID][currAttachmt].Ammo < wAttachments[weapID][currAttachmt].MaxAmmo)
            {
                if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                {
                    LoadAttachmentData(weapID, currAttachmt);

                    PLAY_SOUND(soundID, weaponData[weapID].BuySound);
                    wAttachments[weapID][currAttachmt].Ammo++;
                    WriteIntToINI(wfAttachmentsConfig, (weapID.ToString() + GetAttachmentName(currAttachmt) + "Ammo"), wAttachments[weapID][currAttachmt].Ammo);
                    wfAttachmentsConfig.Save();
                    wfAttachmentsConfig.Load();

                    ADD_SCORE(Main.PlayerIndex, -wAttachments[weapID][currAttachmt].AmmoPrice);
                }
            }
            else if (currAttachmt == 0 && wAttachments[weapID][currAttachmt].Ammo >= wAttachments[weapID][currAttachmt].MaxAmmo)
            {
                IVText.TheIVText.ReplaceTextOfTextLabel("GS1_MONKEY2_02", "~s~Press ~INPUT_FRONTEND_ACCEPT~ to buy ammo. ~n~~r~You can't carry more of that type of ammo.");
                PRINT_HELP("GS1_MONKEY2_02");
                GET_GAME_TIMER(out fTimer);
                bFailMsg = true;
            }
        }

        // LoadWeaponData
        private void LoadGeneralWeaponData(SettingsFile settings, int weapon)
        {
            if (settings.DoesSectionExists(weapon.ToString()))
            {
                string name;
                string model;
                int lockStat;
                float lockProg;
                int wPrice;
                int aPrice;
                int ammo;
                string buySound;
                bool inStock;
                Vector3 backPos;

                name = settings.GetValue(weapon.ToString(), "Name", "");
                if (currEp == 2)
                {
                    model = settings.GetValue(weapon.ToString(), "TBOGTWeaponModel", "");
                    lockStat = settings.GetInteger(weapon.ToString(), "TBOGTUnlockStat", 0);
                    lockProg = settings.GetFloat(weapon.ToString(), "TBOGTUnlockProg", 0);
                }
                else if (currEp == 1)
                {
                    model = settings.GetValue(weapon.ToString(), "TLADWeaponModel", "");
                    lockStat = settings.GetInteger(weapon.ToString(), "TLADUnlockStat", 0);
                    lockProg = settings.GetFloat(weapon.ToString(), "TLADUnlockProg", 0);
                }
                else
                {
                    model = settings.GetValue(weapon.ToString(), "IVWeaponModel", "");
                    lockStat = settings.GetInteger(weapon.ToString(), "IVUnlockStat", 0);
                    lockProg = settings.GetFloat(weapon.ToString(), "IVUnlockProg", 0);
                }
                wPrice = settings.GetInteger(weapon.ToString(), "WeaponPrice", 0);
                aPrice = settings.GetInteger(weapon.ToString(), "AmmoPrice", 0);
                ammo = settings.GetInteger(weapon.ToString(), "ClipAmmo", 0);
                buySound = settings.GetValue(weapon.ToString(), "WeaponBuySound", "");
                inStock = settings.GetBoolean(weapon.ToString(), "BackroomStock", false);
                backPos = settings.GetVector3(weapon.ToString(), "BackroomOffset", Vector3.Zero);

                weaponData[weapon].Name = name;
                weaponData[weapon].Model = model;
                weaponData[weapon].UnlockStat = lockStat;
                weaponData[weapon].UnlockProg = lockProg;
                weaponData[weapon].WeaponPrice = wPrice;
                weaponData[weapon].AmmoPrice = aPrice;
                weaponData[weapon].ClipAmmo = ammo;
                weaponData[weapon].BuySound = buySound;
                weaponData[weapon].BackroomStock = inStock;
                weaponData[weapon].BackroomOffset = backPos;

                if (inStock && !backroomWeaps.Contains(weapon) && IS_MODEL_IN_CDIMAGE(GET_HASH_KEY(weaponData[weapon].Model)))
                    backroomWeaps.Add(weapon);
            }
        }
        private void LoadSpecificWeaponData(SettingsFile settings, int weapon)
        {
            if (settings.DoesSectionExists(weapon.ToString()))
            {
                Vector3 pos;
                Vector3 rot;
                Vector3 bPos;
                float bHdng;
                float bRad;
                int animS;
                int animL;
                int animE;

                pos = settings.GetVector3(weapon.ToString(), "WeaponPosition", Vector3.Zero);
                rot = settings.GetVector3(weapon.ToString(), "WeaponRotation", Vector3.Zero);
                bPos = settings.GetVector3(weapon.ToString(), "BuyPosition", Vector3.Zero);
                bHdng = settings.GetFloat(weapon.ToString(), "BuyHeading", 0);
                bRad = settings.GetFloat(weapon.ToString(), "BuyRadius", 0);
                animS = settings.GetInteger(weapon.ToString(), "AnimStart", 0);
                animL = settings.GetInteger(weapon.ToString(), "AnimLoop", 0);
                animE = settings.GetInteger(weapon.ToString(), "AnimEnd", 0);

                weaponData[weapon].Position = pos;
                weaponData[weapon].Rotation = rot;
                weaponData[weapon].BuyPos = bPos;
                weaponData[weapon].BuyHdng = bHdng;
                weaponData[weapon].BuyRad = bRad;
                weaponData[weapon].AnimStart = animS;
                weaponData[weapon].AnimLoop = animL;
                weaponData[weapon].AnimEnd = animE;
            }
        }

        // LoadINIs
        private void WriteBooleanToINI(SettingsFile settings, string keyname, bool booleShit)
        {
            if (!settings.DoesSectionExists(IVGenericGameStorage.ValidSaveName))
                settings.AddSection(IVGenericGameStorage.ValidSaveName);
            if (!settings.DoesKeyExists(IVGenericGameStorage.ValidSaveName, keyname))
                settings.AddKeyToSection(IVGenericGameStorage.ValidSaveName, keyname);
            settings.SetBoolean(IVGenericGameStorage.ValidSaveName, keyname, booleShit);
        }
        private void WriteIntToINI(SettingsFile settings, string name, int integerShit)
        {
            if (!settings.DoesSectionExists(IVGenericGameStorage.ValidSaveName))
                settings.AddSection(IVGenericGameStorage.ValidSaveName);
            if (!settings.DoesKeyExists(IVGenericGameStorage.ValidSaveName, name))
                settings.AddKeyToSection(IVGenericGameStorage.ValidSaveName, name);
            settings.SetInteger(IVGenericGameStorage.ValidSaveName, name, integerShit);
        }
        private void LoadColors(SettingsFile settings)
        {
            if (currEp == 0)
            {
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
        private void Main_IngameStartup(object sender, EventArgs e)
        {
            hasGameLoaded = false;
        }
        private void OnSaveLoad()
        {
            for (int i = 0; i < gunStores.Count(); i++)
                gunstoreData[i].Hostile = false;

            for (int i = 0; i < numOfWeaponIDs; i++)
            {
                if (attachmentsConfig.DoesSectionExists(i.ToString()))
                {
                    for (int w = 0; w < numOfAttachmts; w++)
                    {
                        if (wAttachments[i][w].Available)
                        {
                            wAttachments[i][w].Owned = attachmentsConfig.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "Has" + GetAttachmentName(w) + "Attachment"), false);
                            wAttachments[i][w].Ammo = attachmentsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, i.ToString() + GetAttachmentName(w) + "Ammo", 0);

                            WriteBooleanToINI(wfAttachmentsConfig, (i.ToString() + "Has" + GetAttachmentName(w) + "Attachment"), wAttachments[i][w].Owned);
                            WriteIntToINI(wfAttachmentsConfig, i.ToString() + GetAttachmentName(w) + "Ammo", wAttachments[i][w].Ammo);
                            wfAttachmentsConfig.Save();
                        }
                    }
                }
            }
        }
        private void Main_Uninitialize(object sender, EventArgs e)
        {
            gunStores.Clear();
            gunstoreData.Clear();
            backroomWeaps.Clear();
            DespawnShit();
            ClearBlips();
        }
        private void Main_Initialized(object sender, EventArgs e)
        {
            LoadSettings(Settings);

            string locPath;
            string[] locConfFiles;

            locPath = string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\LocationData", IVGame.GameStartupPath);
            locConfFiles = System.IO.Directory.GetFiles(locPath);

            foreach (string fileName in locConfFiles)
                gunStores.Add(new SettingsFile(fileName));

            for (int i = 0; i < gunStores.Count(); i++)
            {
                gunStores[i].Load();

                Vector3 loc = gunStores[i].GetVector3("MAIN", "Location", Vector3.Zero);

                string doorHashes = gunStores[i].GetValue("MAIN", "DoorHash", "");
                uint[] doorHash = doorHashes.Split(',').Select(uint.Parse).ToArray();

                string doorXStrng = gunStores[i].GetValue("MAIN", "DoorPosX", "");
                string doorYStrng = gunStores[i].GetValue("MAIN", "DoorPosY", "");
                string doorZStrng = gunStores[i].GetValue("MAIN", "DoorPosZ", "");
                float[] doorPosX = doorXStrng.Split(',').Select(float.Parse).ToArray();
                float[] doorPosY = doorYStrng.Split(',').Select(float.Parse).ToArray();
                float[] doorPosZ = doorZStrng.Split(',').Select(float.Parse).ToArray();

                string dealerMdl = gunStores[i].GetValue("MAIN", "DealerModel", "");
                Vector3 dealerPos = gunStores[i].GetVector3("MAIN", "DealerPosition", Vector3.Zero);
                float dealerHdg = gunStores[i].GetFloat("MAIN", "DealerHeading", 0);
                int dealerWpn = gunStores[i].GetInteger("MAIN", "DealerWeapon", 0);

                string guardString = gunStores[i].GetValue("MAIN", "GuardModels", "");
                string[] guardMdls = guardString.Split(',').ToArray();

                string guardWeaps = gunStores[i].GetValue("MAIN", "GuardWeapons", "");
                int[] guardWpns = guardWeaps.Split(',').Select(int.Parse).ToArray();

                Vector3 guardPos = gunStores[i].GetVector3("MAIN", "GuardSpawnLoc", Vector3.Zero);
                string roomName = gunStores[i].GetValue("MAIN", "RoomName", "");
                string gRoomName = gunStores[i].GetValue("MAIN", "GuardRoomName", "");

                Vector3 backroomPos = gunStores[i].GetVector3("MAIN", "BackroomPos", Vector3.Zero);
                float backroomHdg = gunStores[i].GetFloat("MAIN", "BackroomHdg", 0);
                int iconDisplay = gunStores[i].GetInteger("MAIN", "IconUnlock", -1);

                gunstoreData.Add(new GunStore(loc, false, 0, iconDisplay, 0, doorPosX, doorPosY, doorPosZ, doorHash, dealerMdl, dealerPos, dealerHdg, dealerWpn, guardMdls, guardWpns, guardPos, roomName, gRoomName, backroomPos, backroomHdg));
            }

            weaponProps = new int[numOfWeaponIDs];
            guardPeds = new int[4];
        }
        private void LoadSettings(SettingsFile settings)
        {
            enable = settings.GetBoolean("MAIN", "Enable", false);
            showBuyMarkers = settings.GetBoolean("MAIN", "ShowMarkers", false);
            unlockDYHP = settings.GetBoolean("MAIN", "UnlockAfterDYHP", false);
            buyAmmoEarly = settings.GetBoolean("MAIN", "BuyAmmoBeforeUnlock", false);
            numOfWeaponIDs = settings.GetInteger("MAIN", "WeaponIDAmt", 60);
            extraStock = settings.GetBoolean("MAIN", "ExtraStock", false);
            keepAmmo = settings.GetBoolean("MAIN", "KeepAmmo", false);
            cooldown = settings.GetInteger("MAIN", "HostileCooldown", 0);
            gSpawnDelay = settings.GetInteger("MAIN", "GuardSpawnDelay", 0);

            weaponData = new WeaponHelper.Weapon[numOfWeaponIDs];
            weaponStat = new WeaponHelper.WeaponStat[numOfWeaponIDs];
            wAttachments = new WeaponHelper.Attachment[numOfWeaponIDs][];

            SettingsFile statConfFile = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\WeaponStats.ini", IVGame.GameStartupPath));
            statConfFile.Load();
            SettingsFile msgConfFile = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\WeaponMessages.ini", IVGame.GameStartupPath));
            msgConfFile.Load();
            attachmentsConfig = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\ImprovedGunStores\\Attachments.ini", IVGame.GameStartupPath));
            attachmentsConfig.Load();
            if (System.IO.File.Exists(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath)))
                wfAttachmentsConfig = new SettingsFile(string.Format("{0}\\IVSDKDotNet\\scripts\\WeapFuncs\\Attachments.ini", IVGame.GameStartupPath));
            else
                wfAttachmentsConfig = attachmentsConfig;
            wfAttachmentsConfig.Load();

            for (int i = 0; i < numOfWeaponIDs; i++)
            {
                if (settings.DoesSectionExists(i.ToString()))
                {
                    weaponData[i] = new WeaponHelper.Weapon();
                    weaponStat[i] = new WeaponHelper.WeaponStat();
                    wAttachments[i] = new WeaponHelper.Attachment[numOfAttachmts];
                    for (int w = 0; w < numOfAttachmts; w++)
                    {
                        wAttachments[i][w] = new WeaponHelper.Attachment();
                        wAttachments[i][w].Available = attachmentsConfig.GetBoolean(i.ToString(), "CanBuy" + GetAttachmentName(w) + "Attachment", false);

                        if (wAttachments[i][w].Available)
                        {
                            wAttachments[i][w].Price = attachmentsConfig.GetInteger(i.ToString(), GetAttachmentName(w) + "Price", 0);
                            wAttachments[i][w].AmmoPrice = attachmentsConfig.GetInteger(i.ToString(), GetAttachmentName(w) + "AmmoPrice", 0);
                            wAttachments[i][w].MaxAmmo = attachmentsConfig.GetInteger(i.ToString(), "Max" + GetAttachmentName(w) + "Ammo", 0);

                            wAttachments[i][w].Owned = attachmentsConfig.GetBoolean(IVGenericGameStorage.ValidSaveName, (i.ToString() + "Has" + GetAttachmentName(w) + "Attachment"), false);
                            wAttachments[i][w].Ammo = attachmentsConfig.GetInteger(IVGenericGameStorage.ValidSaveName, i.ToString() + GetAttachmentName(w) + "Ammo", 0);

                            WriteBooleanToINI(wfAttachmentsConfig, (i.ToString() + "Has" + GetAttachmentName(w) + "Attachment"), wAttachments[i][w].Owned);
                            WriteIntToINI(wfAttachmentsConfig, i.ToString() + GetAttachmentName(w) + "Ammo", wAttachments[i][w].Ammo);
                            wfAttachmentsConfig.Save();
                        }
                    }
                    LoadGeneralWeaponData(Settings, i);

                    weaponStat[i].StatTextA = statConfFile.GetValue(i.ToString(), "StatTextA", "");
                    weaponStat[i].StatTextB = statConfFile.GetValue(i.ToString(), "StatTextB", "");
                    weaponStat[i].StatTextC = statConfFile.GetValue(i.ToString(), "StatTextC", "");
                    weaponStat[i].StatTextD = statConfFile.GetValue(i.ToString(), "StatTextD", "");
                    weaponStat[i].StatTextE = statConfFile.GetValue(i.ToString(), "StatTextE", "");
                    weaponStat[i].StatTextF = statConfFile.GetValue(i.ToString(), "StatTextF", "");
                    weaponStat[i].StatTextG = statConfFile.GetValue(i.ToString(), "StatTextG", "");
                    weaponStat[i].StatTextH = statConfFile.GetValue(i.ToString(), "StatTextH", "");

                    weaponStat[i].StatA = statConfFile.GetInteger(i.ToString(), "StatA", -1); ;
                    weaponStat[i].StatB = statConfFile.GetInteger(i.ToString(), "StatB", -1); ;
                    weaponStat[i].StatC = statConfFile.GetInteger(i.ToString(), "StatC", -1); ;
                    weaponStat[i].StatD = statConfFile.GetInteger(i.ToString(), "StatD", -1); ;
                    weaponStat[i].StatE = statConfFile.GetInteger(i.ToString(), "StatE", -1); ;
                    weaponStat[i].StatF = statConfFile.GetInteger(i.ToString(), "StatF", -1); ;
                    weaponStat[i].StatG = statConfFile.GetInteger(i.ToString(), "StatG", -1); ;
                    weaponStat[i].StatH = statConfFile.GetInteger(i.ToString(), "StatH", -1); ;

                    weaponStat[i].BarA = statConfFile.GetInteger(i.ToString(), "BarA", -1);
                    weaponStat[i].BarB = statConfFile.GetInteger(i.ToString(), "BarB", -1);
                    weaponStat[i].BarC = statConfFile.GetInteger(i.ToString(), "BarC", -1);
                    weaponStat[i].BarD = statConfFile.GetInteger(i.ToString(), "BarD", -1);
                    weaponStat[i].BarE = statConfFile.GetInteger(i.ToString(), "BarE", -1);
                    weaponStat[i].BarF = statConfFile.GetInteger(i.ToString(), "BarF", -1);
                    weaponStat[i].BarG = statConfFile.GetInteger(i.ToString(), "BarG", -1);
                    weaponStat[i].BarH = statConfFile.GetInteger(i.ToString(), "BarH", -1);

                    weaponStat[i].Message = msgConfFile.GetValue(i.ToString(), "Message", "");
                }
            }
        }

        // Main Tick
        private bool InitialChecks()
        {
            if (IS_SCREEN_FADED_OUT()) return false;
            if (IS_PAUSE_MENU_ACTIVE()) return false;
            return true;
        }
        public void Main_Tick(object sender, EventArgs e)
        {
            if (enable && (currEp != 0 || GET_FLOAT_STAT(8) > 20 || !unlockDYHP) && !NativeGame.IsScriptRunning("faustin2"))
            {
                PlayerPed = IVPed.FromUIntPtr(IVPlayerInfo.FindThePlayerPed());
                PlayerHandle = PlayerPed.GetHandle();
                PlayerIndex = (int)GET_PLAYER_ID();
                PlayerPos = PlayerPed.Matrix.Pos;

                if (!InitialChecks())
                    return;
                if (PlayerPed == null)
                    return;

                if (!hasGameLoaded)
                {
                    currEp = GET_CURRENT_EPISODE();
                    OnSaveLoad();
                    LoadSettings(Settings);
                    LoadColors(Settings);
                    hasGameLoaded = true;
                }
                if (NativeGame.IsScriptRunning("gunlockup"))
                    TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("gunlockup");
                if (NativeGame.IsScriptRunning("gunlockupct"))
                    TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("gunlockupct");

                GET_GAME_TIMER(out gTimer);

                for (int i = 0; i < gunStores.Count(); i++)
                {
                    ProcessBlips(i);
                    if (LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, gunstoreData[i].Location.X, gunstoreData[i].Location.Y, gunstoreData[i].Location.Z, 50, 50, 50, false))
                    {
                        if (!hasConfigsLoaded)
                        {
                            //gunstoreIndex = i;
                            currGunStore = gunstoreData[i];
                            location = currGunStore.Location;
                            if (gTimer >= (currGunStore.AttackTimer + cooldown))
                                currGunStore.Hostile = false;
                            if (IS_WANTED_LEVEL_GREATER(Main.PlayerIndex, 0))
                            {
                                for (int w = 0; w < currGunStore.DoorHash.Count(); w++)
                                {
                                    SET_STATE_OF_CLOSEST_DOOR_OF_TYPE((uint)currGunStore.DoorHash[w], currGunStore.DoorPosX[w], currGunStore.DoorPosY[w], currGunStore.DoorPosZ[w], true, 0.0f);
                                }
                            }

                            for (int w = 0; w < numOfWeaponIDs; w++)
                            {
                                LoadSpecificWeaponData(gunStores[i], w);
                            }
                            ReplaceText();

                            hasConfigsLoaded = true;
                        }
                        break;
                    }
                    else if (!LOCATE_CHAR_ANY_MEANS_3D(Main.PlayerHandle, location.X, location.Y, location.Z, 50, 50, 50, false) && hasConfigsLoaded)
                    {
                        DespawnShit();
                        RemoveShit();
                        hasSpawned = false;
                        hasConfigsLoaded = false;
                    }
                }
                SpawnGuns();
                BuyGuns();
                SpawnBackroomWeapon();
                ProcessPurchase();
                StopText();
                AnimCheck();
                SpawnDealer();
                SpawnGuards();
                ProcessGuards();
                ProcessInspectAnims();
                ProcessInspectionCam();

                if (DID_SAVE_COMPLETE_SUCCESSFULLY() && GET_IS_DISPLAYINGSAVEMESSAGE())
                {
                    for (int i = 0; i < numOfWeaponIDs; i++)
                    {
                        if (attachmentsConfig.DoesSectionExists(i.ToString()))
                        {
                            for (int w = 0; w < numOfAttachmts; w++)
                            {
                                if (wAttachments[i][w].Available)
                                {
                                    WriteBooleanToINI(attachmentsConfig, (i.ToString() + "Has" + GetAttachmentName(w) + "Attachment"), wAttachments[i][w].Owned);
                                    WriteIntToINI(attachmentsConfig, (i.ToString() + GetAttachmentName(w) + "Ammo"), wAttachments[i][w].Ammo);
                                }
                            }
                        }
                    }

                    attachmentsConfig.Save();
                }
            }
        }
        private class GunStore
        {
            public Vector3 Location { get; set; }
            public uint[] DoorHash { get; set; }
            public float[] DoorPosX { get; set; }
            public float[] DoorPosY { get; set; }
            public float[] DoorPosZ { get; set; }
            public bool Hostile { get; set; }
            public uint AttackTimer { get; set; }
            public int IconDisplay { get; set; }
            public int Blip { get; set; }

            public string DealerModel { get; set; }
            public Vector3 DealerPos { get; set; }
            public float DealerHdng { get; set; }
            public int DealerWpn { get; set; }

            public string[] GuardModels { get; set; }
            public int[] GuardWpns { get; set; }
            public Vector3 GuardPos { get; set; }
            public string RoomName { get; set; }
            public string GuardRoom { get; set; }

            public Vector3 BackroomPos { get; set; }
            public float BackroomHdng { get; set; }
            public GunStore(Vector3 position, bool hostile, uint attackTimer, int iconDisplay, int blip, float[] doorPosX, float[] doorPosY, float[] doorPosZ, uint[] doorHash, string dealerMdl, Vector3 dealerPos, float dealerHdng, int dealerWpn, string[] guardMdls, int[] guardWpns, Vector3 guardPos, string room, string guardRoom, Vector3 backroomPos, float backroomHdng)
            {
                Location = position;
                Hostile = hostile;
                AttackTimer = attackTimer;
                IconDisplay = iconDisplay;
                Blip = blip;
                DoorPosX = doorPosX;
                DoorPosY = doorPosY;
                DoorPosZ = doorPosZ;
                DoorHash = doorHash;

                DealerModel = dealerMdl;
                DealerPos = dealerPos;
                DealerHdng = dealerHdng;
                DealerWpn = dealerWpn;

                GuardModels = guardMdls;
                GuardWpns = guardWpns;
                GuardPos = guardPos;
                RoomName = room;
                GuardRoom = guardRoom;

                BackroomPos = backroomPos;
                BackroomHdng = backroomHdng;
            }
        }
    }
}
