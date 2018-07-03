using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    [System.Serializable]
    public struct Status
    {
        public float HP;
        public float ST;
        public float AT;
        public float DF;
        
        public Status(float HP, float ST, float AT, float DF)
        {
            this.HP = HP;
            this.ST = ST;
            this.AT = AT;
            this.DF = DF;
        }

        public static Status operator +(Status a, Status b)
        {
            Status status = new Status();
            status.HP = a.HP + b.HP;
            status.ST = a.ST + b.ST;
            status.AT = a.AT + b.AT;
            status.DF = a.DF + b.DF;
            return status;
        }

        public static Status operator -(Status a, Status b)
        {
            Status status = new Status();
            status.HP = a.HP - b.HP;
            status.ST = a.ST - b.ST;
            status.AT = a.AT - b.AT;
            status.DF = a.DF - b.DF;
            return status;
        }
    }

    public struct StatusPoint
    {
        public int HP;
        public int ST;
        public int AT;
        public int DF;

        public void Init()
        {
            HP = 1;
            ST = 1;
            AT = 1;
            DF = 1;
        }

        public static Status operator +(Status a, StatusPoint b)
        {
            Status status = new Status();
            status.HP = a.HP + (b.HP * 35);
            status.ST = a.ST + (b.ST * 10);
            status.AT = a.AT + (b.AT * 3);
            status.DF = a.DF + (b.DF * 7);
            return status;
        }

        public static Status operator -(Status a, StatusPoint b)
        {
            Status status = new Status();
            status.HP = a.HP - (b.HP * 35);
            status.ST = a.ST - (b.ST * 10);
            status.AT = a.AT - (b.AT * 3);
            status.DF = a.DF - (b.DF * 7);
            return status;
        }
    }
}

