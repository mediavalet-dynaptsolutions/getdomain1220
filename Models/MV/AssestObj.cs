using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models.MV
{
    



    public class MetaInformation
    {
        public int count { get; set; }
        public int offset { get; set; }
        public string filters { get; set; }
        public string sortPath { get; set; }
        public int elapsedTimeInMS { get; set; }
    }

    public class Meta
    {
        public MetaInformation metaInformation { get; set; }
        public string createdAt { get; set; }
    }

    public class RecordCount
    {
        public int totalRecordsFound { get; set; }
        public int startingRecord { get; set; }
        public int recordsReturned { get; set; }
    }

    public class Rating
    {
        public int average { get; set; }
        public int user { get; set; }
    }

    public class Attributes
    {
        public string __invalid_name__b92f130c_f195_45e7_8bad_a4174aa93111 { get; set; }
    public string __invalid_name__2edf1c71_50e1_4785_a384_3e7bbd3ea18e { get; set; }
public string __invalid_name__75e21bf1_52a5_448a_81a6_6ee48d2311bf { get; set; }
    public string __invalid_name__de7f9cb0_98a6_4800_9516_d0fcacc9f7a0 { get; set; }
    public string __invalid_name__5780c256_bc4d_460d_8263_30c0cdc8795b { get; set; }
    public string __invalid_name__5880780e_5ec8_45df_b9e5_1717647cf995 { get; set; }
    public string __invalid_name__2c4bc745_cbc8_43a6_ac2c_90c055d489fb { get; set; }
    public string __invalid_name__69b67a48_e127_450b_a331_aadf88dd9346 { get; set; }
    public string __invalid_name__784e87d5_b211_4833_8b45_b4d20354a3d0 { get; set; }
    public string __invalid_name__7b221a02_6e96_4418_a5c0_b856b6e5c735 { get; set; }
    public string __invalid_name__939a389a_fcaa_4641_99f0_3c8536addd94 { get; set; }
    public string __invalid_name__b52fe832_9517_4e29_ba8b_cf6019da90f3 { get; set; }
    public string __invalid_name__b53db024_4f94_462b_9f7e_ab62d313604b { get; set; }
}

public class Version
{
    public int version { get; set; }
    public string current { get; set; }
    public string head { get; set; }
    public string parentid { get; set; }
    public bool isversionable { get; set; }
    public bool islatestversion { get; set; }
    public bool ismajorversion { get; set; }
}

public class CreatedBy
{
    public string id { get; set; }
    public string username { get; set; }
}

public class ModifiedBy
{
    public string id { get; set; }
    public string username { get; set; }
}

public class Record
{
    public Version version { get; set; }
    public string createdAt { get; set; }
    public CreatedBy createdBy { get; set; }
    public string modifiedAt { get; set; }
    public ModifiedBy modifiedBy { get; set; }
}

public class Links
{
    public string self { get; set; }
    public List<string> functions { get; set; }
}

public class File
{
    public string title { get; set; }
    public string fileName { get; set; }
    public string fileType { get; set; }
    public int sizeInBytes { get; set; }
    public string description { get; set; }
    public string keywords { get; set; }
    public string uploadedAt { get; set; }
    public string modifiedAt { get; set; }
    public string approvedAt { get; set; }
    public int imageHeight { get; set; }
    public int imageWidth { get; set; }
}

public class Media
{
    public string type { get; set; }
    public string small { get; set; }
    public string thumb { get; set; }
    public string large { get; set; }
    public string original { get; set; }
    public string medium { get; set; }
    public string download { get; set; }
    public string originalDimensions { get; set; }
    public int sasExpiry { get; set; }
    public int sasRenewal { get; set; }
}

public class Asset
{
    public string containerName { get; set; }
    public int status { get; set; }
    public Rating rating { get; set; }
    public Attributes attributes { get; set; }
    public Record record { get; set; }
    public bool renderAsPdf { get; set; }
    public string stream { get; set; }
    public Links _links { get; set; }
    public string id { get; set; }
    public string title { get; set; }
    public File file { get; set; }
    public Media media { get; set; }
}

public class Payload
{
    public int assetCount { get; set; }
    public List<Asset> assets { get; set; }
}

public class MVAssestObj
{
    public string apiVersion { get; set; }
    public Meta meta { get; set; }
    public RecordCount recordCount { get; set; }
    public List<string> warnings { get; set; }
    public Payload payload { get; set; }
}
}