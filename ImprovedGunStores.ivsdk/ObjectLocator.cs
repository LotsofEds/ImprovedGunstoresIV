using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System.Numerics;
using CCL;
using IVSDKDotNet.Enums;
using System.Threading;
using System.Runtime;
using CCL.GTAIV;
using System;
using CCL.GTAIV.Extensions;

namespace ImprovedGunStores.ivsdk
{
    internal class ObjectLocator
    {
        private static float wAnimTime;
        private static float pAnimTime;
        private static float pTime;
        private static float weapX, weapY, weapZ;
        private static float wRotX, wRotY, wRotZ;
        private static Vector3 rot;
        public static Vector3 ToRotation(float X, float Y, float Z, float W)
        {
            double num = W;
            double num2 = X;
            double num3 = Y;
            double num4 = Z;
            double y = ((double)Z * (double)Y + (double)X * (double)W) * 2.0;
            double num5 = num;
            double num6 = num5 * num5;
            double num7 = num2;
            double num8 = num6 - num7 * num7;
            double num9 = num3;
            double num10 = num8 - num9 * num9;
            double num11 = num4;
            float radians = (float)Math.Atan2(y, num10 + num11 * num11);
            num2 = X;
            num = W;
            num3 = Y;
            num4 = Z;
            double y2 = ((double)Y * (double)X + (double)Z * (double)W) * 2.0;
            double num12 = num2;
            double num13 = num12 * num12;
            double num14 = num;
            double num15 = num13 + num14 * num14;
            double num16 = num3;
            double num17 = num15 - num16 * num16;
            double num18 = num4;
            float radians2 = (float)Math.Atan2(y2, num17 - num18 * num18);
            float radians3 = (float)Math.Asin(((double)Z * (double)X - (double)Y * (double)W) * -2.0);
            return new Vector3(Helper.RadianToDegree(radians), Helper.RadianToDegree(radians3), Helper.RadianToDegree(radians2));
        }
        public static void Tick()
        {
            foreach (var obj in ObjectHelper.ObjHandles)
            {
                int objHandle = obj.Value;

                GET_OBJECT_COORDINATES(objHandle, out float objX, out float objY, out float objZ);

                GET_DISTANCE_BETWEEN_COORDS_3D(Main.PlayerPos.X, Main.PlayerPos.Y, Main.PlayerPos.Z, objX, objY, objZ, out float Dist);
                if (Dist > 1.2)
                    continue;

                GET_OBJECT_MODEL(objHandle, out uint pModel);

                //Glock
                //(pModel == 4098655133)
                //Deagle
                //(pModel == 2238994841)
                //Shotgun
                //(pModel == 1846597315)
                //Baretta
                //(pModel == 3719476653)
                //Uzi
                //(pModel == 2949832827)
                //MP5
                //(pModel == 170842493)
                //AK47
                //(pModel == 467469635)
                //M4
                //(pModel == 897930585)
                //PSG1
                //(pModel == 583488944)
                //M40
                //(pModel == 141961522)
                //RPG
                //(pModel == 1443084780)
                //Rocket
                //(pModel == 1516578222)
                //Grenade
                //(pModel == 993473937)
                //Molotov
                //(pModel == 2293515785)
                //Knife
                //(pModel == 1040104843)
                //Bat
                //(pModel == 1758564455)
                GET_OBJECT_COORDINATES(objHandle, out weapX, out weapY, out weapZ);
                GET_OBJECT_QUATERNION(objHandle, out float qX, out float qY, out float qZ, out float qW);
                rot = ToRotation(qX, qY, qZ, qW);

                IVGame.ShowSubtitleMessage(pModel.ToString() + "  " + new Vector3(weapX, weapY, weapZ).ToString() + "  " + rot.ToString());
                //IVGame.ShowSubtitleMessage(pModel.ToString() + "  " + new Vector3(objX, objY, objZ).ToString());
            }
        }
    }
}
