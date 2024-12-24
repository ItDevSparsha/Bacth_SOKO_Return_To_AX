
namespace ListofOrderModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ListofOrder
    {
        [JsonProperty("list_return")]
        public list_return[] list_return { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }
    }
    public partial class list_return
    {
        [JsonProperty("return_id")]
        public string return_id { get; set; }

        [JsonProperty("order_number")]
        public string order_number { get; set; }
        
        [JsonProperty("merchant")]
        public string merchant { get; set; }
        
        [JsonProperty("type_cancel")]
        public string type_cancel { get; set; }
        
        [JsonProperty("return_code")]
        public string return_code { get; set; }

        [JsonProperty("order_code")]
        public string order_code { get; set; }
        
        [JsonProperty("customer_name")]
        public string customer_name { get; set; }
        
        [JsonProperty("qty")]
        public string qty { get; set; }

        [JsonProperty("fee")]
        public string fee { get; set; }

        [JsonProperty("received")]
        public string received { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

    }

    //แต่ละ Order
    public partial class Get_Order_Return
    {
        [JsonProperty("detail_order")]
        public detail_order detail_order { get; set; }
        [JsonProperty("detail_item")]
        public detail_item[] detail_item { get; set; }
        [JsonProperty("file_upload")]
        public file_upload[] file_upload { get; set; }
    }

    public partial class detail_order
    {
        [JsonProperty("return_id")]
        public string return_id { get; set; }

        [JsonProperty("order_number")]
        public string order_number { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("customer_name")]
        public string customer_name { get; set; }

        [JsonProperty("customer_address")]
        public string customer_address { get; set; }

        [JsonProperty("customer_phone")]
        public string customer_phone { get; set; }

        [JsonProperty("charge_rate")]
        public string charge_rate { get; set; }

        [JsonProperty("received_date")]
        public string received_date { get; set; }

        [JsonProperty("merchant")]
        public string merchant { get; set; }

        [JsonProperty("order_code")]
        public string order_code { get; set; }

        [JsonProperty("remark")]
        public string remark { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("return_status")]
        public string return_status { get; set; }
    }
    public partial class detail_item
    {
        [JsonProperty("i_id")]
        public string i_id { get; set; }

        [JsonProperty("i_name")]
        public string i_name { get; set; }

        [JsonProperty("item_sku")]
        public string item_sku { get; set; }

        [JsonProperty("item_quantity")]
        public string item_quantity { get; set; }

        [JsonProperty("item_return_comment")]
        public string item_return_comment { get; set; }

        [JsonProperty("sokochan_code")]
        public string sokochan_code { get; set; }

        [JsonProperty("status_item")]
        public string status_item { get; set; }

    }
    public partial class file_upload
    {
        [JsonProperty("return_id")]
        public string return_id { get; set; }

        [JsonProperty("path_file")]
        public string path_file { get; set; }

    }
    //เก่า//
    public partial class Order
    {
        [JsonProperty("external_id")]
        public object ExternalId { get; set; }

        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("order_number")]
        public string OrderNumber { get; set; }

        [JsonProperty("comment")]
        public object Comment { get; set; }

        [JsonProperty("special_order")]
        public object SpecialOrder { get; set; }

        [JsonProperty("representative")]
        public string Representative { get; set; }

        [JsonProperty("cod_amount")]
        public string CodAmount { get; set; }

        [JsonProperty("insert_qty")]
        public long InsertQty { get; set; }

        [JsonProperty("wrap")]
        public long Wrap { get; set; }

        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        [JsonProperty("tracking_no")]
        public string TrackingNo { get; set; }

        [JsonProperty("package")]
        public object Package { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("shipping_status")]
        public string ShippingStatus { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("updated")]
        public DateTimeOffset Updated { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("order_items")]
        public OrderItem[] OrderItems { get; set; }

        [JsonProperty("order_history")]
        public OrderHistory[] OrderHistory { get; set; }

        [JsonProperty("billing")]
        public System.Collections.Generic.Dictionary<string, long> Billing { get; set; }
    }

    public partial class Customer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("postal_code")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string PostalCode { get; set; }

        [JsonProperty("mobile_no")]
        public string MobileNo { get; set; }

        [JsonProperty("phone_no")]
        public object PhoneNo { get; set; }

        [JsonProperty("email")]
        public object Email { get; set; }
    }

    public partial class OrderHistory
    {
        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }

    public partial class OrderItem
    {
        [JsonProperty("item_number")]
        public string ItemNumber { get; set; }

        [JsonProperty("item_sku")]
        public string ItemSku { get; set; }

        [JsonProperty("item_code")]
        public string ItemCode { get; set; }

        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("item_qty")]
        public decimal ItemQty { get; set; }
    }

    



        public partial class Get_Order
    {
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("external_id")]
        public object external_id { get; set; }

        [JsonProperty("data")]
        public Data data { get; set; }

        [JsonProperty("request_id")]
        public string request_id { get; set; }

    }
    public partial class Data
    {
        [JsonProperty("external_id")]
        public object externalId { get; set; }

        [JsonProperty("order_number")]
        public string order_number { get; set; }

        [JsonProperty("order_code")]
        public string order_code { get; set; }

        [JsonProperty("store")]
        public object store { get; set; }

        [JsonProperty("special_order")]
        public object special_order { get; set; }

        [JsonProperty("comment")]
        public string comment { get; set; }

        [JsonProperty("shipping")]
        public string shipping { get; set; }

        [JsonProperty("shipping_name")]
        public string shipping_name { get; set; }

        [JsonProperty("tracking_no")]
        public string tracking_no { get; set; }

        [JsonProperty("weight")]
        public long weight { get; set; }

        [JsonProperty("MKTPL_Create")]
        public string MKTPL_Create { get; set; }

        [JsonProperty("MKTPL_Update")]
        public string MKTPL_Update { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("created")]
        //public DateTimeOffset created { get; set; }
        public DateTime created { get; set; }

        [JsonProperty("updated")]
        //public DateTimeOffset updated { get; set; }
        public DateTime updated { get; set; }

        [JsonProperty("store_name")]
        public object store_name { get; set; }

        [JsonProperty("channel_name")]
        public string channel_name { get; set; }

        [JsonProperty("customer")]
        public customer_detail customer { get; set; }

        [JsonProperty("payment")]
        public payment_detail payment { get; set; }

        [JsonProperty("order_items")]
        public order_items_detail[] order_items { get; set; }

        [JsonProperty("order_items_shipped")]
        public order_items_shipped_detail[] order_items_shipped { get; set; }

        [JsonProperty("order_history")]
        public order_history_detail[] order_history { get; set; }

        [JsonProperty("attached_files")]
        public attached_files_detail[] attached_files { get; set; }
    }

    public partial class attached_files_detail
    {

    }

    public partial class order_history_detail
    {
        [JsonProperty("date_time")]
        public string date_time { get; set; }

        [JsonProperty("location")]
        public string location { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("remark")]
        public string remark { get; set; }

        [JsonProperty("timestamp")]
        public long timestamp { get; set; }
    }


    public partial class order_items_shipped_detail
    {
        [JsonProperty("item_sku")]
        public string item_sku { get; set; }

        [JsonProperty("item_code")]
        public string item_code { get; set; }

        [JsonProperty("marketplace_sku")]
        public string marketplace_sku { get; set; }

        [JsonProperty("item_name")]
        public string item_name { get; set; }

        [JsonProperty("item_qty")]
        public string item_qty { get; set; }

        [JsonProperty("item_cost")]
        public string item_cost { get; set; }

        [JsonProperty("item_selling")]
        public string item_selling { get; set; }
    }


    public partial class order_items_detail
    {
        [JsonProperty("item_sku")]
        public string item_sku { get; set; }

        [JsonProperty("item_code")]
        public string item_code { get; set; }

        [JsonProperty("marketplace_sku")]
        public string marketplace_sku { get; set; }

        [JsonProperty("item_name")]
        public string item_name { get; set; }

        [JsonProperty("item_qty")]
        public string item_qty { get; set; }

        [JsonProperty("selling_price")]
        public string selling_price { get; set; }

        [JsonProperty("paid_price")]
        public string paid_price { get; set; }

        [JsonProperty("voucher_platform")]
        public string voucher_platform { get; set; }

        [JsonProperty("voucher_seller")]
        public string voucher_seller { get; set; }

        [JsonProperty("cost_price_warehouse")]
        public string cost_price_warehouse { get; set; }

        [JsonProperty("selling_price_warehouse")]
        public string selling_price_warehouse { get; set; }
    }

    public partial class payment_detail
    {
        [JsonProperty("shipping_fee_original")]
        public string channshipping_fee_originalel_name { get; set; }

        [JsonProperty("shipping_fee_discount_platform")]
        public string shipping_fee_discount_platform { get; set; }

        [JsonProperty("shipping_fee_discount_seller")]
        public string shipping_fee_discount_seller { get; set; }

        [JsonProperty("shipping_fee")]
        public string shipping_fee { get; set; }

        [JsonProperty("voucher_platform")]
        public string voucher_platform { get; set; }

        [JsonProperty("voucher_seller")]
        public string voucher_seller { get; set; }

        [JsonProperty("voucher_amount")]
        public string voucher_amount { get; set; }

        [JsonProperty("price")]
        public string price { get; set; }

        [JsonProperty("cod_amount")]
        public string cod_amount { get; set; }
    }


    public partial class customer_detail
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("sub_district")]
        public string sub_district { get; set; }

        [JsonProperty("district")]
        public string district { get; set; }

        [JsonProperty("province")]
        public string province { get; set; }

        [JsonProperty("postal_code")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string postal_code { get; set; }

        [JsonProperty("mobile_no")]
        public string mobile_no { get; set; }

        [JsonProperty("phone_no")]
        public object phone_no { get; set; }

        [JsonProperty("email")]
        public object email { get; set; }
    }

    public partial class Paging
    {
        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public enum Item { Test001, Test002, Test003 };

    public partial class Welcome7
    {
        public static Welcome7 FromJson(string json) => JsonConvert.DeserializeObject<Welcome7>(json, ListofOrderModel.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome7 self) => JsonConvert.SerializeObject(self, ListofOrderModel.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ItemConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }


        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class ItemConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Item) || t == typeof(Item?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "test001":
                    return Item.Test001;
                case "test002":
                    return Item.Test002;
                case "test003":
                    return Item.Test003;
            }
            throw new Exception("Cannot unmarshal type Item");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Item)untypedValue;
            switch (value)
            {
                case Item.Test001:
                    serializer.Serialize(writer, "test001");
                    return;
                case Item.Test002:
                    serializer.Serialize(writer, "test002");
                    return;
                case Item.Test003:
                    serializer.Serialize(writer, "test003");
                    return;
            }
            throw new Exception("Cannot marshal type Item");
        }

        public static readonly ItemConverter Singleton = new ItemConverter();
    }
}
