using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedGunStores.ivsdk
{
    internal class WeaponHelper
    {
        public class Weapon
        {
            public string Name;
            public string Model;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 BuyPos;
            public float BuyHdng;
            public float BuyRad;
            public int AnimStart;
            public int AnimLoop;
            public int AnimEnd;
            public int UnlockStat;
            public float UnlockProg;
            public int WeaponPrice;
            public int AmmoPrice;
            public int ClipAmmo;
            public string BuySound;
            public bool BackroomStock;
            public Vector3 BackroomOffset;

            public Weapon()
            {
            }
        }
        public class WeaponStat
        {
            public int Pellets;

            public string Message;
            public string StatTextA;
            public string StatTextB;
            public string StatTextC;
            public string StatTextD;
            public string StatTextE;
            public string StatTextF;
            public string StatTextG;
            public string StatTextH;

            public int StatA;
            public int StatB;
            public int StatC;
            public int StatD;
            public int StatE;
            public int StatF;
            public int StatG;
            public int StatH;

            public int BarA;
            public int BarB;
            public int BarC;
            public int BarD;
            public int BarE;
            public int BarF;
            public int BarG;
            public int BarH;

            public WeaponStat()
            {
            }
        }
        public class Attachment
        {
            public string Name;
            public int Price;
            public int AmmoPrice;
            public int Ammo;
            public int MaxAmmo;
            public bool Available;
            public bool Owned;
            public Attachment()
            {
            }
        }
    }
}
