using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon
{
    public class ProductResponseEntity
    {
        public List<ProductModel> Data { get; set; }
        public int Count { get; set; }

    }
    public class Root
    {
        public ProductModel Data { get; set; }
    }

    public class ProductModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Name { get; set; }       
        public string Images { get; set; }
        public string Description { get; set; }
        public Style Style { get; set; }
        public List<Components> Components { get; set; }
        public Size Size { get; set; }
        public string FunctionType { get; set; }
        public bool IsSharedModel { get; set; }
        public long CategoryId { get; set; }
        public List<CustomAttributes> CustomAttributes { get; set; }
        public bool IsVirtualProduct { get; set; }
        public int OwnerId { get; set; }
        public string CreatedUtc { get; set; }
        public string UpdatedUtc { get; set; }
        public bool Published { get; set; }
        public bool Latest { get; set; }
        public int Revision { get; set; }
        public bool IsLiked { get; set; }
        public int LikedNumber { get; set; }
        public bool IsCollected { get; set; }
        public int CollectedNumber { get; set; }
        public string ExtraData { get; set; }
        public long BoundId { get; set; }
        public string Tags { get; set; }
        public List<int> TagIds { get; set; }
        public bool Listable { get; set; }

    }
    public class Style
    {
        public int style { get; set; }
        public int Complexity { get; set; }
        public int Metalness { get; set; }
        public int Roughness { get; set; }
        public int Size { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }

    }

    public class Components
    {
        public int Id { get; set; }
        public int FromId { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public Material Material { get; set; }
        public string ExtraData { get; set; }
        public string CreatedUtc { get; set; }
        public string UpdatedUtc { get; set; }
    }

    public class Material
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string Name { get; set; }
        public string Images { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public string Params { get; set; }
        public Dna Dna { get; set; }
        public List<CustomAttributes> CustomAttributes { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public string CreatedUtc { get; set; }
        public string UpdatedUtc { get; set; }       
        public bool Latest { get; set; }
        public int Revision { get; set; }
        public bool IsLiked { get; set; }
        public int LikedNumber { get; set; }
        public bool IsCollected { get; set; }
        public int CollectedNumber { get; set; }
        public string ExtraData { get; set; }
    }
    public class Dna
    {
        public int Style { get; set; }
        public int Complexity { get; set; }
        public int Metalness { get; set; }
        public int Roughness { get; set; }
        public int Size { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }

    }

    public class Size
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public class CustomAttributes
    {
        //public decimal bPrice { get; set; }
        //public bool cListable { get; set; }
        //public string tTaobaoLink { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string ValueType { get; set; }

        public string Value { get; set; }
    }


}
