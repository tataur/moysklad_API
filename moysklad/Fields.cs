using System;
using System.Collections.Generic;

namespace moysklad
{
    //контрагент
    public class Contr
    {
        public string name { get; set; }
    }


    //отгрузка
    public class Demand
    {
        public string name { get; set; }
        public string moment { get; set; }
        /*public string created { get; set; }
        public string updated { get; set; }*/
        public Organization organization { get; set; }
        public Agent agent { get; set; }
        public Store store { get; set; }
    }

    public class Demands
    {
        public List<RowDemand> rows { get; set; }
    }

    public class RowDemand
    {
        public string id { get; set; }
        public string name { get; set; }
        public string moment { get; set; }
    }


    public class Organization
    {
        public Meta meta { get; set; }
    }

    public class Agent
    {
        public Meta meta { get; set; }
    }

    public class Store
    {
        public Meta meta { get; set; }
    }


    public class Meta
    {
        public string href { get; set; }
        public string type { get; set; }
        public string mediaType { get; set; }
    }


    public class JsonDemand
    {
        public string id { get; set; }
    }


    public class Position
    {
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public Assortment assortment { get; set; }
    }

    public class Assortment
    {
        public Meta meta{get; set; }
    }


    public class Product
    {
/*        public string context { get; set; }
        public Meta meta { get; set; }*/
        public List<RowProduct> rows { get; set; }
    }

    public class RowProduct
    {
        public string id { get; set; }
        public string name { get; set; }
    }


    public class DemandsFromCSV
    {
        public string RealizationDate { get; set; } //demandDate
        public string NumberDoc { get; set; } //damendName
        public string ProductNameReal { get; set; } //productName
        public string CustomerName { get; set; } //agentName
        public string VolumeReal { get; set; } //position.quantity
        public string SellPrice { get; set; } //position.price
    }
}
