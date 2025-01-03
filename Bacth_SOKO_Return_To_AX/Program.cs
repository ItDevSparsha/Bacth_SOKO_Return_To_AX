using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata.Ecma335;
using ListofOrderModel;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Net;

namespace Bacth_SOKO_Return_To_AX
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            try
            {
                String conBI = ConfigurationManager.AppSettings["conBI"];

                var client = new HttpClient();
                var date = DateTime.Now;
                var day = date.Day.ToString();
                var month = date.Month.ToString();
                var year = date.Year.ToString();
                var today_date = year + '-' + month + '-' + day;
                //ดึงย้อนหลังตามจำนวนวันที่ fix ไว้
                var _day = Convert.ToInt32(ConfigurationManager.AppSettings["day"]);

                for (var i = 0; i <= _day; i++)

                //เปิดตัวนี้ถ้าเลือกดึงวันเดียว
                //for (var i = 0; i < 1; i++)
                {
                    var fix_day = date.AddDays(1 * (-1));
                    Console.WriteLine("Today is " + fix_day.Day.ToString() + '-' + fix_day.Month.ToString() + '-' + fix_day.Year.ToString());
                    var _date = fix_day.Year.ToString() + '-' + fix_day.Month.ToString() + '-' + fix_day.Day.ToString();
                    Console.WriteLine("กำลังทำของวันที่ " + _date);
                    //เส้นจริง
                    var _url = "https://verite.sokochan.com/api/1.0/returns?received_date=" + _date ;
                    //เส้นเทส
                    //var _url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns?received_date=" + _date ;

                    //เปิดตัวนี้ถ้าเลือกวันจุดที่ 1 (มี 2 จุด)
                    //เส้นจริง
                    //var _url = "https://verite.sokochan.com/api/1.0/returns?received_date=2024-12-24";
                    //เส้นเทส
                    //var _url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns?received_date=2024-12-03";

                    var request = new HttpRequestMessage(HttpMethod.Get, _url);
                    request.Headers.Add("Authorization", "Basic " + svcCredentials);
                    var content = new StringContent("", null, "text/plain");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    else
                    {
                        Console.WriteLine("ไม่มีข้อมูล");
                        Environment.Exit(0);
                    }
                    //ได้ headder มา
                    dynamic jsonObj = JsonConvert.DeserializeObject<ListofOrderModel.ListofOrder>(await response.Content.ReadAsStringAsync());

                    /*เรื่อง offset*/
                    var api_total = jsonObj.Paging.Total;
                    var max_api_offset = jsonObj.Paging.Total - jsonObj.Paging.Limit; //1-100
                    var round_max_api_offset = jsonObj.Paging.Total / jsonObj.Paging.Limit; //1/100
                    var round = round_max_api_offset;
                    if (round_max_api_offset > 0) //ดัก Order ที่น้อยกว่า 100
                    {
                        round = round_max_api_offset * 100;
                    }
                    else
                    {
                        round = 1;
                    }
                    for (var offset = 0; offset <= round; offset += 100) //เปิดวน offset
                    {
                        //เส้นจริง
                        var new_url = "https://verite.sokochan.com/api/1.0/returns?received_date=" + _date ;
                        //เส้นเทส
                        //var new_url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns?received_date=" + _date ;

                        //ทำเผื่อเรื่อง offset ตอนนี้เส้น api ยังไม่ทำให้
                        //เส้นจริง
                        //var new_url = "https://verite.sokochan.com/api/1.0/returns?received_date=" + _date+ "&offset=" + offset ;
                        //เส้นเทส
                        //var new_url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns?received_date=" + _date + "&offset=" + offset ;

                        //เปิดตัวนี้ถ้าเลือกวันจุดที่ 2
                        //เส้นจริง
                        //var new_url = "https://verite.sokochan.com/api/1.0/returns?received_date=2024-12-24";
                        //เส้นเทส
                        //var new_url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns?received_date=2024-12-03";

                        var new_request = new HttpRequestMessage(HttpMethod.Get, new_url);
                        new_request.Headers.Add("Authorization", "Basic " + svcCredentials);
                        var new_content = new StringContent("", null, "text/plain");
                        new_request.Content = new_content;
                        var new_response = await client.SendAsync(new_request);
                        if (new_response.IsSuccessStatusCode)
                        {
                            new_response.EnsureSuccessStatusCode();
                        }
                        else
                        {
                            Console.WriteLine("ไม่มีข้อมูล");
                            Environment.Exit(0);
                        }
                        jsonObj = JsonConvert.DeserializeObject<ListofOrderModel.ListofOrder>(await new_response.Content.ReadAsStringAsync());

                        foreach (var order in jsonObj.list_return)
                        {
                            if (order.status == "Restocked" && order.type_cancel == "Non-Delivery")
                            {
                                Console.WriteLine("order_number : " + order.order_number);
                                Console.WriteLine("return_id : " + order.return_id);

                                //ปิดไว้กันยิงเส้นไม่เจอ ให้ไปหาในเบสก่อน ถ้ามีค่อยยิงเส้น api
                                //var return_id = order.return_id.ToString();
                                //var url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns/" + return_id;
                                ////ใช้ในกรณีเลือก return_id
                                ////var url = "https://verite.sokochan.com/api/1.0/orders/2409-002-00925";
                                //var request_getorderreturn = new HttpRequestMessage(HttpMethod.Get, url);
                                //request_getorderreturn.Headers.Add("Authorization", "Basic " + svcCredentials);
                                //var content_getorder = new StringContent("", null, "text/plain");
                                //request_getorderreturn.Content = content_getorder;
                                //var response_getorder = await client.SendAsync(request_getorderreturn);
                                //response_getorder.EnsureSuccessStatusCode();
                                //dynamic jsonObj_getorderreturn = JsonConvert.DeserializeObject<ListofOrderModel.Get_Order_Return>(await response_getorder.Content.ReadAsStringAsync());

                                /*Connect to Base*/
                                Console.WriteLine();
                                //String conBI = ConfigurationManager.AppSettings["conBI"];

                                //หา order_number และ return_id
                                String SQL_return = "SELECT * FROM T_ORDER_MARKETPLACE WHERE ORDER_NUMBER = '" + order.order_number + "' AND RETURN_ID = N'" + order.return_id  + "'";
                                DataTable order_number_return = new DataTable();
                                order_number_return = QueryDT(conBI, SQL_return);

                                //หา order_number
                                String SQL = "SELECT * FROM T_ORDER_MARKETPLACE WHERE ORDER_NUMBER = '" + order.order_number + "'";
                                DataTable order_number = new DataTable();
                                order_number = QueryDT(conBI, SQL);

                                //เก็บข้อมูลที่เป็น Header 
                                String SQL_Header = "SELECT * FROM T_ORDER_MARKETPLACE WHERE ORDER_STATUS NOT IN (N'RET') AND ORDER_NUMBER = '" + order.order_number + "'";
                                DataTable order_number_header = new DataTable();
                                order_number_header = QueryDT(conBI, SQL_Header);

                                //เก็บค่า INTERFACE_STATUS && ORDER_STATUS 
                                String ORDER_STATUS_In = "RET";
                                String INTERFACE_STATUS_In = "WAI";
                                String ORDER_STATUS_Up = "RET";

                                if ((order_number.Rows[0]["INTERFACE_STATUS"].ToString() == "DRA" || order_number.Rows[0]["INTERFACE_STATUS"].ToString() == "WAI")
                                  && (order_number.Rows[0]["ORDER_STATUS"].ToString() == "SHI"|| order_number.Rows[0]["ORDER_STATUS"].ToString() == "CAN"))
                                {
                                    ORDER_STATUS_In     = "CAN";
                                    INTERFACE_STATUS_In = "WAI";
                                    ORDER_STATUS_Up     = "CAN";
                                }
                                else if (order_number.Rows[0]["INTERFACE_STATUS"].ToString() == "COM"
                                      && order_number.Rows[0]["ORDER_STATUS"].ToString() == "SHI")
                                {
                                    ORDER_STATUS_In     = "RET";
                                    INTERFACE_STATUS_In = "WAI";
                                    ORDER_STATUS_Up     = "SHI";
                                }
                                else if (order_number.Rows[0]["INTERFACE_STATUS"].ToString() == "INC"
                                      && order_number.Rows[0]["ORDER_STATUS"].ToString() == "SHI")
                                {
                                    ORDER_STATUS_In     = "RET";
                                    INTERFACE_STATUS_In = "DRA";
                                    ORDER_STATUS_Up     = "SHI";
                                }

                                //เก็บข้อมูลที่เป็น REASON_CODE
                                String SQL_REASON_CODE = "SELECT * FROM M_CHANNEL_CONFIG WHERE CON_CODE_2 = N'REASON_CODE'";
                                DataTable orderdb_reason_code = new DataTable();
                                orderdb_reason_code = QueryDT(conBI, SQL_REASON_CODE);
                                string reason_code_name = "";
                                foreach (DataRow row in orderdb_reason_code.Rows)
                                {
                                    if (row["CON_CODE_1"].ToString() == jsonObj.list_return[0].type_cancel)
                                    {
                                        reason_code_name = row["CON_VALUE"].ToString();
                                    }
                                }

                                //เก็บข้อมูลที่เป็น WAREHOUSE_CODE
                                String SQL_WAREHOUSE_CODE = "SELECT * FROM M_CHANNEL_CONFIG WHERE CON_CODE_2 = N'WAREHOUSE_CODE'";
                                DataTable orderdb_warehouse_code = new DataTable();
                                orderdb_warehouse_code = QueryDT(conBI, SQL_WAREHOUSE_CODE);
                                string warehouse_code_name = "";
                                foreach (DataRow row in orderdb_warehouse_code.Rows)
                                {
                                    if (row["CON_CODE_1"].ToString() == jsonObj.list_return[0].type_cancel)
                                    {
                                        warehouse_code_name = row["CON_VALUE"].ToString();
                                    }
                                }
                                //เก็บ error 
                                var error = "0";
                                //String or = order_number_header.Rows[0]["ORDER_NUMBER"].ToString();
                                //String da = jsonObj_getorderreturn.detail_order.received_date; 
                                //if ให้ไปหาในเบสก่อน ถ้ามีค่อยยิงเส้น api
                                if (order_number.Rows.Count > 0)
                                {
                                    Console.WriteLine("มี order_number ใน base");
                                    var return_id = order.return_id.ToString();
                                    //เส้นจริง
                                    var url = "https://verite.sokochan.com/api/1.0/returns/" + return_id;
                                    //เส้นเทส
                                    //var url = "https://dev99.sokojung.com/uat-verite/api/1.0/returns/" + return_id;
                                    //ใช้ในกรณีเลือก return_id
                                    //var url = "https://verite.sokochan.com/api/1.0/returns/113";

                                    var request_getorderreturn = new HttpRequestMessage(HttpMethod.Get, url);
                                    request_getorderreturn.Headers.Add("Authorization", "Basic " + svcCredentials);
                                    var content_getorder = new StringContent("", null, "text/plain");
                                    request_getorderreturn.Content = content_getorder;
                                    var response_getorder = await client.SendAsync(request_getorderreturn);
                                    response_getorder.EnsureSuccessStatusCode();
                                    dynamic jsonObj_getorderreturn = JsonConvert.DeserializeObject<ListofOrderModel.Get_Order_Return>(await response_getorder.Content.ReadAsStringAsync());



                                    if (order_number.Rows.Count == 1 && order_number_return.Rows.Count == 0)//ยังไม่เคยมีการส่งคืน และต้องไม่ลงซ้ำ
                                    {
                                        Console.WriteLine("ยังไม่เคยมีการส่งคืน");
                                        var uid_market = Guid.NewGuid().ToString("N");
                                        StringBuilder insertSQL = new StringBuilder();
                                        insertSQL.Clear();
                                        insertSQL.Append("INSERT INTO T_ORDER_MARKETPLACE ");
                                        insertSQL.Append("( ");
                                        insertSQL.Append("UID, ");
                                        insertSQL.Append("ORDER_NUMBER, ");
                                        insertSQL.Append("CREATED_DATE, ");
                                        insertSQL.Append("ORDER_STATUS, ");
                                        insertSQL.Append("CUSTOMER_ACCOUNT, ");
                                        insertSQL.Append("CUSTOMER_MOBILE, ");
                                        insertSQL.Append("CUSTOMER_PHONE, ");
                                        insertSQL.Append("CUSTOMER_EMAIL, ");
                                        insertSQL.Append("DELIVERY_NAME, ");
                                        insertSQL.Append("DELIVERY_DISTRICT, ");
                                        insertSQL.Append("DELIVERY_PROVINCE, ");
                                        insertSQL.Append("DELIVERY_POSTAL_CODE, ");
                                        insertSQL.Append("CHANNEL_NAME, ");
                                        insertSQL.Append("COMP_CODE , ");
                                        insertSQL.Append("REQUESTED_SHIP_DATE, ");
                                        insertSQL.Append("TERM_OF_PAYMENT, ");
                                        insertSQL.Append("WAREHOUSE_CODE, ");
                                        insertSQL.Append("AX_SO_NUMBER, ");
                                        insertSQL.Append("INS_DATE, ");
                                        insertSQL.Append("INS_BY, ");
                                        insertSQL.Append("UPD_DATE, ");
                                        insertSQL.Append("UPD_BY, ");
                                        insertSQL.Append("INTERFACE_STATUS, ");
                                        insertSQL.Append("INTERFACE_ERROR_LOG, ");
                                        insertSQL.Append("AX_INVOICE_NUMBER, ");
                                        insertSQL.Append("AX_PICKING_NUMBER, ");
                                        insertSQL.Append("AX_PACKING_NUMBER, ");
                                        insertSQL.Append("TAX_ID, ");
                                        insertSQL.Append("SELLER_DISCOUNT, ");
                                        insertSQL.Append("SHOPEE_DISCOUNT, ");
                                        insertSQL.Append("UID_REQUEST_ETAX, ");
                                        insertSQL.Append("REF_AX_SO_NUMBER, ");
                                        insertSQL.Append("REF_AX_INVOICE_NUMBER, ");
                                        insertSQL.Append("REF_AX_PICKING_NUMBER, ");
                                        insertSQL.Append("REF_AX_PACKING_NUMBER, ");
                                        insertSQL.Append("REASON_CODE, ");
                                        insertSQL.Append("RETURN_REMARK, ");
                                        insertSQL.Append("UID_MARKETPLACE, ");
                                        insertSQL.Append("TAX_TYPE, ");
                                        insertSQL.Append("CARD_TYPE, ");
                                        insertSQL.Append("RETURN_ID, ");
                                        insertSQL.Append("BRANCH_NO ");
                                        insertSQL.Append(") ");

                                        insertSQL.Append("VALUES( ");
                                        insertSQL.AppendFormat("N'{0}', ", uid_market);
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["ORDER_NUMBER"].ToString());
                                        insertSQL.AppendFormat("'{0}' , ", jsonObj_getorderreturn.detail_order.received_date);
                                        //insertSQL.AppendFormat("N'RET', ");
                                        insertSQL.AppendFormat("N'{0}', ", ORDER_STATUS_In);
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_ACCOUNT"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_MOBILE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_PHONE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_EMAIL"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_NAME"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_DISTRICT"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_PROVINCE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_POSTAL_CODE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CHANNEL_NAME"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["COMP_CODE"].ToString());
                                        //insertSQL.AppendFormat("'{0}' , ", order_number_header.Rows[0]["REQUESTED_SHIP_DATE"].ToString());
                                        insertSQL.AppendFormat("'{0}' , ", jsonObj_getorderreturn.detail_order.received_date);
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TERM_OF_PAYMENT"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", warehouse_code_name);
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("GETDATE(), ");
                                        insertSQL.AppendFormat("N'SOKO', ");
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("NULL, ");
                                        //insertSQL.AppendFormat("N'WAI', ");
                                        insertSQL.AppendFormat("N'{0}', ", INTERFACE_STATUS_In);
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("NULL, ");
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TAX_ID"].ToString());
                                        insertSQL.AppendFormat("0, ");
                                        insertSQL.AppendFormat("0, ");
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["UID_REQUEST_ETAX"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_SO_NUMBER"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_INVOICE_NUMBER"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_PICKING_NUMBER"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_PACKING_NUMBER"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", reason_code_name);
                                        insertSQL.AppendFormat("N'{0}', ", jsonObj_getorderreturn.detail_order.remark);
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["UID"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TAX_TYPE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CARD_TYPE"].ToString());
                                        insertSQL.AppendFormat("N'{0}', ", jsonObj_getorderreturn.detail_order.return_id);
                                        insertSQL.AppendFormat("N'{0}' ", order_number_header.Rows[0]["BRANCH_NO"].ToString());
                                        insertSQL.Append(") ");
                                        //ปิดส่งเบสก่อน
                                        NonQuery(conBI, insertSQL.ToString());
                                        //update saleorder
                                        try
                                        {
                                            var uid_Up = order_number_header.Rows[0]["UID"].ToString();
                                            string _Update = $@"UPDATE T_ORDER_MARKETPLACE
                                                               SET ORDER_STATUS  = N'{ORDER_STATUS_Up}'
                                                               WHERE  UID = N'{uid_Up}'
                                                                ";
                                            DataTable queryUp1 = new DataTable();
                                            queryUp1 = QueryDT(conBI, _Update);
                                        }
                                        catch (Exception ex)
                                        {
                                            error = "1";
                                            throw new ArgumentException(ex.Message);
                                        }
                                        //วนไอเทม
                                        foreach (var item in jsonObj_getorderreturn.detail_item)
                                        {
                                            if (item.item_sku.Substring(0, 3) == "GWP") //ไม่คิดของแถม ปล.ของแถมขึ้นต้นด้วย GWP
                                            {
                                                item.item_sku = item.item_sku.Replace("GWP", "");
                                            }
                                            String SQL_DETAIL = "SELECT * FROM T_ORDER_MARKETPLACE_DETAIL WHERE UID_ORDER_MARKETPLACE = N'" + order_number_header.Rows[0]["UID"].ToString() + "' AND ITEM_NUMBER = N'" + item.item_sku + "'";
                                            DataTable item_detail = new DataTable();
                                            item_detail = QueryDT(conBI, SQL_DETAIL);

                                            double _QTY = Math.Round(Convert.ToDouble(item.item_quantity), 2) * (-1);
                                            double _NET_AMOUNT = Math.Round(Convert.ToDouble(item_detail.Rows[0]["UNIT_PRICE"]), 6) * Math.Round(Convert.ToDouble(item.item_quantity), 6);

                                            if (item_detail.Rows.Count > 0)//เป็น item ที่ return
                                            {
                                                Console.WriteLine("item ที่ return : " + item.item_sku);
                                                insertSQL.Clear();
                                                insertSQL.Append("INSERT INTO T_ORDER_MARKETPLACE_DETAIL ");
                                                insertSQL.Append("( ");
                                                insertSQL.Append("UID, ");
                                                insertSQL.Append("UID_ORDER_MARKETPLACE, ");
                                                insertSQL.Append("ITEM_NUMBER, ");
                                                insertSQL.Append("ITEM_NAME, ");
                                                insertSQL.Append("DIM_SIZE, ");
                                                insertSQL.Append("DIM_COLOR, ");
                                                insertSQL.Append("DIM_STYLE, ");
                                                insertSQL.Append("QTY, ");
                                                insertSQL.Append("UNIT_PRICE, ");
                                                insertSQL.Append("DISCOUNT_BHT, ");
                                                insertSQL.Append("NET_AMOUNT, ");
                                                insertSQL.Append("COST_CENTER, ");
                                                insertSQL.Append("DEPARTMENT, ");
                                                insertSQL.Append("TAX_BRANCH, ");
                                                insertSQL.Append("ORIGINAL_NET_AMOUNT, ");
                                                insertSQL.Append("SELLING_DISCOUNT, ");
                                                insertSQL.Append("SHOPEE_DISCOUNT, ");
                                                insertSQL.Append("WEIGHT_PRICE ");
                                                insertSQL.Append(") ");

                                                insertSQL.Append("VALUES( ");
                                                insertSQL.AppendFormat("N'{0}', ", Guid.NewGuid().ToString("N"));
                                                insertSQL.AppendFormat("N'{0}', ", uid_market);
                                                insertSQL.AppendFormat("N'{0}', ", item.item_sku);
                                                insertSQL.AppendFormat("N'{0}', ", item.i_name);
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_SIZE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_COLOR"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_STYLE"].ToString());
                                                insertSQL.AppendFormat("'{0}', ", Math.Round(_QTY, 2));
                                                if (string.IsNullOrEmpty(item_detail.Rows[0]["UNIT_PRICE"].ToString()))
                                                {
                                                    insertSQL.AppendFormat("NULL, ");
                                                }
                                                else
                                                {
                                                    insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["UNIT_PRICE"]);
                                                }
                                                //insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["UNIT_PRICE"].ToString());
                                                insertSQL.AppendFormat("0, ");
                                                insertSQL.AppendFormat("'{0}', ", Math.Round(_NET_AMOUNT, 6));
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["COST_CENTER"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DEPARTMENT"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["TAX_BRANCH"].ToString());
                                                if (string.IsNullOrEmpty(item_detail.Rows[0]["ORIGINAL_NET_AMOUNT"].ToString()))
                                                {
                                                    insertSQL.AppendFormat("NULL, ");
                                                }
                                                else
                                                {
                                                    insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["ORIGINAL_NET_AMOUNT"]);
                                                }
                                                if (string.IsNullOrEmpty(item_detail.Rows[0]["SELLING_DISCOUNT"].ToString()))
                                                {
                                                    insertSQL.AppendFormat("NULL, ");
                                                }
                                                else
                                                {
                                                    insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SELLING_DISCOUNT"]);
                                                }
                                                if (string.IsNullOrEmpty(item_detail.Rows[0]["SHOPEE_DISCOUNT"].ToString()))
                                                {
                                                    insertSQL.AppendFormat("NULL, ");
                                                }
                                                else
                                                {
                                                    insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SHOPEE_DISCOUNT"]);
                                                }
                                                if (string.IsNullOrEmpty(item_detail.Rows[0]["WEIGHT_PRICE"].ToString()))
                                                {
                                                    insertSQL.AppendFormat("NULL ");
                                                }
                                                else
                                                {
                                                    insertSQL.AppendFormat("'{0}' ", item_detail.Rows[0]["WEIGHT_PRICE"]);
                                                }
                                                //insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["ORIGINAL_NET_AMOUNT"]);
                                                //insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SELLING_DISCOUNT"]);
                                                //insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SHOPEE_DISCOUNT"]);
                                                //insertSQL.AppendFormat("'{0}' ", item_detail.Rows[0]["WEIGHT_PRICE"]);
                                                insertSQL.Append(") ");
                                                //ปิดส่งเบสก่อน
                                                NonQuery(conBI, insertSQL.ToString());
                                            }
                                        }
                                    }
                                    else if (order_number.Rows.Count > 1 && order_number_return.Rows.Count == 0)//เคยมีการส่งคืนแล้ว และต้องไม่ลงซ้ำ
                                    {
                                        foreach (DataRow row in order_number.Rows)
                                        {
                                            if (row["RETURN_ID"].ToString() == return_id)//เช็คว่าเคยเก็บค่า return_id นี้ไปแล้วหรือยัง
                                            {
                                            }
                                            else
                                            {
                                                Console.WriteLine("เคยมีการส่งคืนแล้ว");
                                                var uid_market = Guid.NewGuid().ToString("N");
                                                StringBuilder insertSQL = new StringBuilder();
                                                insertSQL.Clear();
                                                insertSQL.Append("INSERT INTO T_ORDER_MARKETPLACE ");
                                                insertSQL.Append("( ");
                                                insertSQL.Append("UID, ");
                                                insertSQL.Append("ORDER_NUMBER, ");
                                                insertSQL.Append("CREATED_DATE, ");
                                                insertSQL.Append("ORDER_STATUS, ");
                                                insertSQL.Append("CUSTOMER_ACCOUNT, ");
                                                insertSQL.Append("CUSTOMER_MOBILE, ");
                                                insertSQL.Append("CUSTOMER_PHONE, ");
                                                insertSQL.Append("CUSTOMER_EMAIL, ");
                                                insertSQL.Append("DELIVERY_NAME, ");
                                                insertSQL.Append("DELIVERY_DISTRICT, ");
                                                insertSQL.Append("DELIVERY_PROVINCE, ");
                                                insertSQL.Append("DELIVERY_POSTAL_CODE, ");
                                                insertSQL.Append("CHANNEL_NAME, ");
                                                insertSQL.Append("COMP_CODE , ");
                                                insertSQL.Append("REQUESTED_SHIP_DATE, ");
                                                insertSQL.Append("TERM_OF_PAYMENT, ");
                                                insertSQL.Append("WAREHOUSE_CODE, ");
                                                insertSQL.Append("AX_SO_NUMBER, ");
                                                insertSQL.Append("INS_DATE, ");
                                                insertSQL.Append("INS_BY, ");
                                                insertSQL.Append("UPD_DATE, ");
                                                insertSQL.Append("UPD_BY, ");
                                                insertSQL.Append("INTERFACE_STATUS, ");
                                                insertSQL.Append("INTERFACE_ERROR_LOG, ");
                                                insertSQL.Append("AX_INVOICE_NUMBER, ");
                                                insertSQL.Append("AX_PICKING_NUMBER, ");
                                                insertSQL.Append("AX_PACKING_NUMBER, ");
                                                insertSQL.Append("TAX_ID, ");
                                                insertSQL.Append("SELLER_DISCOUNT, ");
                                                insertSQL.Append("SHOPEE_DISCOUNT, ");
                                                insertSQL.Append("UID_REQUEST_ETAX, ");
                                                insertSQL.Append("REF_AX_SO_NUMBER, ");
                                                insertSQL.Append("REF_AX_INVOICE_NUMBER, ");
                                                insertSQL.Append("REF_AX_PICKING_NUMBER, ");
                                                insertSQL.Append("REF_AX_PACKING_NUMBER, ");
                                                insertSQL.Append("REASON_CODE, ");
                                                insertSQL.Append("RETURN_REMARK, ");
                                                insertSQL.Append("UID_MARKETPLACE, ");
                                                insertSQL.Append("TAX_TYPE, ");
                                                insertSQL.Append("CARD_TYPE, ");
                                                insertSQL.Append("RETURN_ID, ");
                                                insertSQL.Append("BRANCH_NO ");
                                                insertSQL.Append(") ");

                                                insertSQL.Append("VALUES( ");
                                                insertSQL.AppendFormat("N'{0}', ", uid_market);
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["ORDER_NUMBER"].ToString());
                                                insertSQL.AppendFormat("'{0}' , ", jsonObj_getorderreturn.detail_order.received_date);
                                                //insertSQL.AppendFormat("N'RET', ");
                                                insertSQL.AppendFormat("N'{0}', ", ORDER_STATUS_In);
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_ACCOUNT"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_MOBILE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_PHONE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CUSTOMER_EMAIL"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_NAME"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_DISTRICT"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_PROVINCE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["DELIVERY_POSTAL_CODE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CHANNEL_NAME"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["COMP_CODE"].ToString());
                                                //insertSQL.AppendFormat("'{0}' , ", order_number_header.Rows[0]["REQUESTED_SHIP_DATE"].ToString());
                                                insertSQL.AppendFormat("'{0}' , ", jsonObj_getorderreturn.detail_order.received_date);
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TERM_OF_PAYMENT"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", warehouse_code_name);
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("GETDATE(), ");
                                                insertSQL.AppendFormat("N'SOKO', ");
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("NULL, ");
                                                //insertSQL.AppendFormat("N'WAI', ");
                                                insertSQL.AppendFormat("N'{0}', ", INTERFACE_STATUS_In);
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("NULL, ");
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TAX_ID"].ToString());
                                                insertSQL.AppendFormat("0, ");
                                                insertSQL.AppendFormat("0, ");
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["UID_REQUEST_ETAX"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_SO_NUMBER"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_INVOICE_NUMBER"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_PICKING_NUMBER"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["AX_PACKING_NUMBER"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", reason_code_name);
                                                insertSQL.AppendFormat("N'{0}', ", jsonObj_getorderreturn.detail_order.remark);
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["UID"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["TAX_TYPE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", order_number_header.Rows[0]["CARD_TYPE"].ToString());
                                                insertSQL.AppendFormat("N'{0}', ", jsonObj_getorderreturn.detail_order.return_id);
                                                insertSQL.AppendFormat("N'{0}' ", order_number_header.Rows[0]["BRANCH_NO"].ToString());
                                                insertSQL.Append(") ");
                                                //ปิดส่งเบสก่อน
                                                NonQuery(conBI, insertSQL.ToString());
                                                //update saleorder
                                                try
                                                {
                                                    var uid_Up = order_number_header.Rows[0]["UID"].ToString();
                                                    string _Update = $@"UPDATE T_ORDER_MARKETPLACE
                                                               SET ORDER_STATUS  = N'{ORDER_STATUS_Up}'
                                                               WHERE  UID = N'{uid_Up}'
                                                                ";
                                                    DataTable queryUp1 = new DataTable();
                                                    queryUp1 = QueryDT(conBI, _Update);
                                                }
                                                catch (Exception ex)
                                                {
                                                    error = "2";
                                                    throw new ArgumentException(ex.Message);
                                                }
                                                //วนไอเทม
                                                foreach (var item in jsonObj_getorderreturn.detail_item)
                                                {
                                                    if (item.item_sku.Substring(0, 3) == "GWP") //ไม่คิดของแถม ปล.ของแถมขึ้นต้นด้วย GWP
                                                    {
                                                        item.item_sku = item.item_sku.Replace("GWP", "");
                                                    }
                                                    String SQL_DETAIL = "SELECT * FROM T_ORDER_MARKETPLACE_DETAIL WHERE UID_ORDER_MARKETPLACE = N'" + order_number_header.Rows[0]["UID"].ToString() + "' AND ITEM_NUMBER = N'" + item.item_sku + "'";
                                                    DataTable item_detail = new DataTable();
                                                    item_detail = QueryDT(conBI, SQL_DETAIL);

                                                    double _QTY = Math.Round(Convert.ToDouble(item.item_quantity), 2) * (-1);
                                                    double _NET_AMOUNT = Math.Round(Convert.ToDouble(item_detail.Rows[0]["UNIT_PRICE"]), 6) * Math.Round(Convert.ToDouble(item.item_quantity), 6);

                                                    if (item_detail.Rows.Count > 0)//เป็น item ที่ return
                                                    {
                                                        Console.WriteLine("item ที่ return : " + item.item_sku);
                                                        insertSQL.Clear();
                                                        insertSQL.Append("INSERT INTO T_ORDER_MARKETPLACE_DETAIL ");
                                                        insertSQL.Append("( ");
                                                        insertSQL.Append("UID, ");
                                                        insertSQL.Append("UID_ORDER_MARKETPLACE, ");
                                                        insertSQL.Append("ITEM_NUMBER, ");
                                                        insertSQL.Append("ITEM_NAME, ");
                                                        insertSQL.Append("DIM_SIZE, ");
                                                        insertSQL.Append("DIM_COLOR, ");
                                                        insertSQL.Append("DIM_STYLE, ");
                                                        insertSQL.Append("QTY, ");
                                                        insertSQL.Append("UNIT_PRICE, ");
                                                        insertSQL.Append("DISCOUNT_BHT, ");
                                                        insertSQL.Append("NET_AMOUNT, ");
                                                        insertSQL.Append("COST_CENTER, ");
                                                        insertSQL.Append("DEPARTMENT, ");
                                                        insertSQL.Append("TAX_BRANCH, ");
                                                        insertSQL.Append("ORIGINAL_NET_AMOUNT, ");
                                                        insertSQL.Append("SELLING_DISCOUNT, ");
                                                        insertSQL.Append("SHOPEE_DISCOUNT, ");
                                                        insertSQL.Append("WEIGHT_PRICE ");
                                                        insertSQL.Append(") ");

                                                        insertSQL.Append("VALUES( ");
                                                        insertSQL.AppendFormat("N'{0}', ", Guid.NewGuid().ToString("N"));
                                                        insertSQL.AppendFormat("N'{0}', ", uid_market);
                                                        insertSQL.AppendFormat("N'{0}', ", item.item_sku);
                                                        insertSQL.AppendFormat("N'{0}', ", item.i_name);
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_SIZE"].ToString());
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_COLOR"].ToString());
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DIM_STYLE"].ToString());
                                                        insertSQL.AppendFormat("'{0}', ", Math.Round(_QTY, 2));
                                                        if (string.IsNullOrEmpty(item_detail.Rows[0]["UNIT_PRICE"].ToString()))
                                                        {
                                                            insertSQL.AppendFormat("NULL, ");
                                                        }
                                                        else
                                                        {
                                                            insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["UNIT_PRICE"]);
                                                        }
                                                        //insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["UNIT_PRICE"].ToString());
                                                        insertSQL.AppendFormat("0, ");
                                                        insertSQL.AppendFormat("'{0}', ", Math.Round(_NET_AMOUNT, 6));
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["COST_CENTER"].ToString());
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["DEPARTMENT"].ToString());
                                                        insertSQL.AppendFormat("N'{0}', ", item_detail.Rows[0]["TAX_BRANCH"].ToString());
                                                        if (string.IsNullOrEmpty(item_detail.Rows[0]["ORIGINAL_NET_AMOUNT"].ToString()))
                                                        {
                                                            insertSQL.AppendFormat("NULL, ");
                                                        }
                                                        else
                                                        {
                                                            insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["ORIGINAL_NET_AMOUNT"]);
                                                        }
                                                        if (string.IsNullOrEmpty(item_detail.Rows[0]["SELLING_DISCOUNT"].ToString()))
                                                        {
                                                            insertSQL.AppendFormat("NULL, ");
                                                        }
                                                        else
                                                        {
                                                            insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SELLING_DISCOUNT"]);
                                                        }
                                                        if (string.IsNullOrEmpty(item_detail.Rows[0]["SHOPEE_DISCOUNT"].ToString()))
                                                        {
                                                            insertSQL.AppendFormat("NULL, ");
                                                        }
                                                        else
                                                        {
                                                            insertSQL.AppendFormat("'{0}', ", item_detail.Rows[0]["SHOPEE_DISCOUNT"]);
                                                        }
                                                        if (string.IsNullOrEmpty(item_detail.Rows[0]["WEIGHT_PRICE"].ToString()))
                                                        {
                                                            insertSQL.AppendFormat("NULL ");
                                                        }
                                                        else
                                                        {
                                                            insertSQL.AppendFormat("'{0}' ", item_detail.Rows[0]["WEIGHT_PRICE"]);
                                                        }
                                                        insertSQL.Append(") ");
                                                        //ปิดส่งเบสก่อน
                                                        NonQuery(conBI, insertSQL.ToString());
                                                    }//ปิดเป็น item ที่ return
                                                }//ปิดวนไอเทม
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("มี return order_number นี้ใน base แล้ว");
                                    }
                                }//ปิดif ให้ไปหาในเบสก่อน ถ้ามีค่อยยิงเส้น api
                                else
                                {
                                    //order_number นี้ไม่ได้มีอยู่ในระบบไม่ต้องทำอะไร
                                    Console.WriteLine("ไม่มี order_number นี้ใน base");
                                }
                            }//ปิดเช็คสถานะRestocked
                        }//ปิดวนออเดอร์
                    }//ปิดวน offset
                }//ปิดวนวัน

                //เช็คเคส INC ที่ยังไม่ Update
                Console.WriteLine("start check INC not yet update");
                //หา order_number และ return_id
                String SQL_Check = "SELECT *  FROM T_ORDER_MARKETPLACE WHERE INTERFACE_STATUS = N'COM'   AND UID IN ( " +
                                    "  SELECT UID_MARKETPLACE  FROM T_ORDER_MARKETPLACE " +
                                    "  WHERE ORDER_STATUS = N'RET' " +
                                    "  AND INTERFACE_STATUS = N'DRA' " +
                                    "  AND (AX_INVOICE_NUMBER IS NULL OR AX_INVOICE_NUMBER = '' )) ";
                DataTable recheck_order_number_INC = new DataTable();
                recheck_order_number_INC = QueryDT(conBI, SQL_Check);
                if (recheck_order_number_INC.Rows.Count > 0)
                {
                    foreach (DataRow data_row in recheck_order_number_INC.Rows)
                    {
                        var AX_SO_NUMBER = data_row["AX_SO_NUMBER"].ToString();
                        var AX_INVOICE_NUMBER = data_row["AX_INVOICE_NUMBER"].ToString();
                        var AX_PICKING_NUMBER = data_row["AX_PICKING_NUMBER"].ToString();
                        var AX_PACKING_NUMBER = data_row["AX_PACKING_NUMBER"].ToString();
                        var UID_MARKETPLACE = data_row["UID"].ToString();
                        string INC_Update = $@"UPDATE T_ORDER_MARKETPLACE
                                                               SET  REF_AX_SO_NUMBER  = '{AX_SO_NUMBER}',
                                                               REF_AX_INVOICE_NUMBER  = '{AX_INVOICE_NUMBER}',
                                                               REF_AX_PICKING_NUMBER  = '{AX_PICKING_NUMBER}',
                                                               REF_AX_PACKING_NUMBER  = '{AX_PACKING_NUMBER}',
                                                               INTERFACE_STATUS = N'WAI'
                                                               WHERE  UID_MARKETPLACE = N'{UID_MARKETPLACE}'
                                                                ";
                        DataTable Update_INC = new DataTable();
                        Update_INC = QueryDT(conBI, INC_Update);
                    }
                }
                else
                {
                    Console.WriteLine("No INC status that have not been updated.");
                }
                //จบเช็คเคส INC ที่ยังไม่ Update
            }
            catch
            {
                throw;
            }
        }
        public static void NonQuery(String con, String sql)
        {
            using (SqlConnection conn = new SqlConnection(con))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Console.WriteLine("1 :" + ex.Message);
                }
            }
        }

        public static DataTable QueryDT(String con, String sql)
        {
            DataTable db = new DataTable();
            using (SqlConnection conn = new SqlConnection(con))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(db);
                    conn.Close();
                    da.Dispose();
                    //Console.WriteLine("---------------------------"+ newID.ToString());
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Console.WriteLine("2 :" + ex.Message);
                }
            }
            return db;
        }
    }
}
