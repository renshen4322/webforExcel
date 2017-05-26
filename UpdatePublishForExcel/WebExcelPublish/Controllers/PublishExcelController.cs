using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using publishCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebExcelPublish.Controllers
{
    public class PublishExcelController : Controller
    {
        private string tokenUrl = "https://api.vidahouse.com/Token"; //获取key url
        private string allproductUrl = "https://api.vidahouse.com/designing/v1.0/Products?";//查询所有的物品url
        private string allcustomUrl = "https://api.vidahouse.com/designing/v1.0/Attributes";  //获取所有的属性的url       
        private string updateAndPublishUrl = "https://api.vidahouse.com/designing/v1.0/Products/publish";//更新并发布产品url
        private string VersionByIdUrl = "https://api.vidahouse.com/designing/v1.0/Products/version/"; //根据版本id获取该物品的所有详情

        private ServerRequestHelper serverHelper = new ServerRequestHelper();
        private List<ProductModel> listProductEntity = new List<ProductModel>();
        private ConcurrentQueue<ProductModel> _ConcurrenProducts = new ConcurrentQueue<ProductModel>();
        // GET: PublishExcel        
        private int PageSize = 200;
        public ActionResult Index(string strKeyWord = null, string strOrderBy = null, string strOrder = null, string strCategory = null, string strUserIds = null, string strFilter = null, int PageIndex = 1)
        {
            if (Session["Token"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //var listEntity = SearchProductInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter);
            PagerInfo pager = new PagerInfo();
            pager.PageSize = PageSize;
            pager.CurrentPageIndex = PageIndex;
            Dictionary<string, string> httpParams = new Dictionary<string, string>();
            httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
            ProductResponseEntity requstParm = RequestHttpInfo(httpParams);
            List<ProductModel> listResponseEntity = new List<ProductModel>();
            if (requstParm.Count > 0)
            {
                var totalCount = requstParm.Count; //总条数
                var getModule = totalCount % PageSize; //总条数与200取模
                var divCount = totalCount / PageSize; //相除
                var pageCount = (getModule == 0) ? divCount : (divCount + 1); //通过条数获得总页数;
                if (requstParm.Count <= PageSize)
                {
                    httpParams = new Dictionary<string, string>();
                    httpParams.Add("Index", PageIndex.ToString());
                    httpParams.Add("Size", totalCount.ToString());  //总条数不够200条。
                    httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
                    requstParm = RequestHttpInfo(httpParams);
                    AddDataToList(requstParm, listResponseEntity);
                }
                else
                {
                    httpParams = new Dictionary<string, string>();
                    httpParams.Add("Index", PageIndex.ToString());
                    httpParams.Add("Size", PageSize.ToString());
                    httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
                    requstParm = RequestHttpInfo(httpParams);
                    AddDataToList(requstParm, listResponseEntity);
                }
            }
            pager.RecordCount = requstParm.Count;
            //var resultList = listEntity.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            PagerQuery<PagerInfo, IList<ProductModel>> query = new PagerQuery<PagerInfo, IList<ProductModel>>(pager, listResponseEntity);
            ViewBag.DataList = query;
            return View("Index");
        }

        public List<ProductModel> SearchProductInfo(string strKeyWord = "", string strOrderBy = "", string strOrder = "", string strCategory = "", string strUserIds = "", string strFilter = "", int PageIndex = 1)
        {
            Dictionary<string, string> httpParams = new Dictionary<string, string>();
            httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
            ProductResponseEntity requstParm = RequestHttpInfo(httpParams);
            List<ProductModel> listResponseEntity = new List<ProductModel>();
            int Count = 0;
            if (requstParm.Count > 0)
            {
                var totalCount = requstParm.Count; //总条数
                var getModule = totalCount % PageSize; //总条数与200取模
                var divCount = totalCount / PageSize; //相除
                var pageCount = (getModule == 0) ? divCount : (divCount + 1); //通过条数获得总页数;
                if (requstParm.Count <= PageSize)
                {
                    httpParams = new Dictionary<string, string>();
                    httpParams.Add("Index", 1.ToString());
                    httpParams.Add("Size", totalCount.ToString());  //总条数不够200条。
                    httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
                    requstParm = RequestHttpInfo(httpParams);
                    AddDataToList(requstParm, listResponseEntity);
                }
                else
                {
                    int[] pageInfo = new int[pageCount];
                    for (int i = 1; i <= pageInfo.Length; i++)
                    {
                        Count++;
                        httpParams = new Dictionary<string, string>();
                        httpParams.Add("Index", i.ToString());
                        httpParams.Add("Size", PageSize.ToString());
                        httpParams = MergeParmInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter, httpParams);
                        requstParm = RequestHttpInfo(httpParams);
                        AddDataToList(requstParm, listResponseEntity);
                    }
                }

            }
            return listResponseEntity;
        }
        /// <summary>
        /// 添加数据到list
        /// </summary>
        /// <param name="requstParm"></param>
        /// <param name="listResponseEntity"></param>
        private static void AddDataToList(ProductResponseEntity requstParm, List<ProductModel> listResponseEntity)
        {
            foreach (var data in requstParm.Data)
            {
                ProductModel entity = new ProductModel();
                entity.Id = data.Id;
                entity.Name = data.Name;
                entity.Images = data.Images;
                entity.CategoryId = data.CategoryId;
                entity.OwnerId = data.OwnerId;
                entity.Description = data.Description;
                entity.Published = data.Published;
                if (data.CustomAttributes.Count > 0)
                {
                    entity.CustomAttributes = data.CustomAttributes;

                }
                listResponseEntity.Add(entity);
            }
        }

        /// <summary>
        /// 根据参数请求http数据反序列为实体
        /// </summary>
        /// <param name="httpParams"></param>
        /// <returns></returns>
        private ProductResponseEntity RequestHttpInfo(Dictionary<string, string> httpParams)
        {
            var formDataString = HttpRequstHelper.FormatParams(httpParams);
            var url = allproductUrl + formDataString;
            var result = serverHelper.RequestGetReuse(url);
            var requstParm = JsonConvert.DeserializeObject<ProductResponseEntity>(result); //根据查询条件获得总条数，
            return requstParm;
        }

        /// <summary>
        /// 组合参数
        /// </summary>
        /// <param name="strKeyWord"></param>
        /// <param name="strOrderBy"></param>
        /// <param name="strOrder"></param>
        /// <param name="strCategory"></param>
        /// <param name="strUserIds"></param>
        /// <param name="strFilter"></param>
        /// <returns></returns>
        private Dictionary<string, string> MergeParmInfo(string strKeyWord, string strOrderBy, string strOrder, string strCategory, string strUserIds, string strFilter, Dictionary<string, string> httpParams)
        {

            if (!string.IsNullOrEmpty(strKeyWord))
            {
                httpParams.Add("Keyword", strKeyWord);
                ViewBag.KeywordInfo = strKeyWord;
            }
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                httpParams.Add("Orderby", strOrderBy);
                ViewBag.OrderByInfo = strOrderBy;
            }
            else
            {
                httpParams.Add("Orderby", "CreatedUtc");
                ViewBag.OrderByInfo = "CreatedUtc";
            }
            httpParams.Add("Order", strOrder);
            ViewBag.OrderInfo = strOrder;
            if (!string.IsNullOrEmpty(strCategory))
            {
                httpParams.Add("Category", strCategory);
                ViewBag.CategoryInfo = strCategory;
            }
            if (!string.IsNullOrEmpty(strUserIds))
            {
                httpParams.Add("UserIds", strUserIds);
                ViewBag.UserIdsInfo = strUserIds;
            }
            if (!string.IsNullOrEmpty(strFilter))
            {
                httpParams.Add("Filter", strFilter);
                ViewBag.FilterInfo = strFilter;
            }

            return httpParams;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public object GetTokenInfo()
        {
            if (Session["Token"] != null)
            {
                Dictionary<string, string> httpParams = new Dictionary<string, string>();
                httpParams.Add("UserName", "vida001");
                httpParams.Add("grant_type", "password");
                httpParams.Add("Password", "Vida@002");
                Dictionary<string, string> hearderParams = new Dictionary<string, string>();
                var respon = HttpRequstHelper.OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), tokenUrl, httpParams, hearderParams, "application/x-www-form-urlencoded");
                return respon.StatusCode;
            }
            return null;
        }
        /// <summary>
        /// 查询创建的所有的自定义属性
        /// </summary>
        /// <returns></returns>
        public string GetCustomInfo()
        {
            if (Session["Token"] != null)
            {
                var result = serverHelper.RequestGetReuse(allcustomUrl);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 导出组合数据ToExcel
        /// </summary>
        /// <param name="strKeyWord"></param>
        /// <param name="strOrderBy"></param>
        /// <param name="strOrder"></param>
        /// <param name="strCategory"></param>
        /// <param name="strUserIds"></param>
        /// <param name="strFilter"></param>
        /// <returns></returns>
        public ActionResult ExportToExcl(string strKeyWord, string strOrderBy, string strOrder, string strCategory, string strUserIds, string strFilter)
        {
            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("物品详情列表");
            IRow row = sheet.CreateRow(0);
            // 获取样式
            Func<ICellStyle> getCellStyle = () =>
            {
                ICellStyle cellStyle = workBook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.Left;
                return cellStyle;
            };
            ICellStyle headerStyle = getCellStyle();
            IFont fontHeade = workBook.CreateFont();
            fontHeade.FontHeightInPoints = 11;
            fontHeade.FontName = "微软雅黑";
            fontHeade.Boldweight = (short)FontBoldWeight.Normal;
            headerStyle.SetFont(fontHeade);

            Func<string, int, ICell> setCellStyle = (string cellText, int colindex) =>
            {
                ICell cell = row.CreateCell(colindex);
                //sheet.SetColumnWidth(colindex, 9000);
                sheet.AutoSizeColumn(colindex);
                cell.SetCellValue(cellText);
                cell.CellStyle = headerStyle;
                return cell;
            };
            setCellStyle("Name", 0);
            setCellStyle("Id", 1);
            setCellStyle("CategoryID", 2);
            setCellStyle("OwnerId", 3);
            setCellStyle("Images", 4);
            setCellStyle("Description", 5);
            setCellStyle("Price", 6);
            setCellStyle("TaobaoLink", 7);
            setCellStyle("Listable", 8);
            setCellStyle("Published", 9);
            int rowStart = 1;
            var listEntity = SearchProductInfo(strKeyWord, strOrderBy, strOrder, strCategory, strUserIds, strFilter);
            if (listEntity != null)
            {
                foreach (var data in listEntity)
                {
                    IRow myrow = sheet.CreateRow(rowStart);
                    myrow.CreateCell(0).SetCellValue(data.Name);
                    myrow.CreateCell(1).SetCellValue(data.Id);
                    myrow.CreateCell(2).SetCellValue(data.CategoryId);
                    myrow.CreateCell(3).SetCellValue(data.OwnerId);
                    myrow.CreateCell(4).SetCellValue(data.Images);
                    myrow.CreateCell(5).SetCellValue(data.Description);
                    if (data.CustomAttributes == null)
                    {
                        myrow.CreateCell(6).SetCellValue(0.0);
                        myrow.CreateCell(7).SetCellValue("https://www.taobao.com/");
                    }
                    else
                    {
                        myrow.CreateCell(6).SetCellValue(data.CustomAttributes[0].Value);
                        myrow.CreateCell(7).SetCellValue(data.CustomAttributes[1].Value);
                    }
                    myrow.CreateCell(8).SetCellValue(data.Listable.ToString().ToLower());
                    myrow.CreateCell(9).SetCellValue(data.Published.ToString().ToLower());
                    rowStart++;
                }

            }
            using (MemoryStream ms = new MemoryStream())
            {
                workBook.Write(ms);
                byte[] buffers = ms.ToArray();
                return File(buffers, "application/vnd.ms-excel", string.Format("{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

            }
        }
        public ActionResult ImportPage()
        {
            if (Session["Token"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View("ImportPage");
        }
        public delegate string MethodCaller(string name);

        /// <summary>
        /// 导入更新并发布
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import()
        {
            var token = SessionHelper.GetSession("Token");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int stCount = 0;

            #region 检验文件并上传保存
            if (Request.Files[0] == null || Request.Files[0].ContentLength <= 0)
            {
                return Json(new { result = false, message = "文件或数据不能为空" }, "text/html");
            }
            else
            {
                string FileName;
                string savePath;
                string filename = Path.GetFileName(Request.Files[0].FileName);
                int filesize = Request.Files[0].ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                int Maxsize = 20000 * 1024;//定义上传文件的最大空间大小为20M
                string FileType = ".xls,.xlsx";//定义上传文件的类型字符串

                FileName = NoFileName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    return Json(new { result = false, message = ViewBag.error }, "text/html");
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过20M，不能上传";
                    return Json(new { result = false, message = ViewBag.error }, "text/html");
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/excel/";
                if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(path);
                }
                savePath = Path.Combine(path, FileName);
                Request.Files[0].SaveAs(savePath);

            }
            #endregion
            try
            {
                NPOI.SS.UserModel.IWorkbook workBook = new NPOI.HSSF.UserModel.HSSFWorkbook(Request.Files[0].InputStream);
                NPOI.SS.UserModel.ISheet sheet = workBook.GetSheetAt(0);
                List<string> strList = new List<string>();
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    stCount++;
                    NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    var model = new ProductModel();
                    model.Name = row.GetCell(0).ToString().Trim();
                    model.Id = row.GetCell(1).ToString().ToInt32();
                    model.CategoryId = row.GetCell(2).ToString().ToInt32();
                    model.OwnerId = row.GetCell(3) == null ? 0 : row.GetCell(3).ToString().ToInt32();
                    model.Images = row.GetCell(4) == null ? "" : row.GetCell(4).ToString().Trim();
                    model.Description = row.GetCell(5) == null ? "" : row.GetCell(5).ToString().Trim();
                    var custom = new List<CustomAttributes>();
                    var _price = row.GetCell(6) == null ? "" : row.GetCell(6).ToString().Trim();
                    var _taobaoLink = row.GetCell(7) == null ? "" : row.GetCell(7).ToString().Trim();
                    var _listable = row.GetCell(8) == null ? false : row.GetCell(8).ToString().ToBoolean();
                    custom = new List<CustomAttributes>() {
                        new CustomAttributes() { Name = "bPrice", DisplayName = "Price",ValueType="Number",Value = _price },
                        new CustomAttributes() { Name = "tTaobaoLink", DisplayName = "TaobaoLink",ValueType="Text", Value = _taobaoLink  }
                    };
                    model.CustomAttributes = custom;
                    model.Published = row.GetCell(9) == null ? false : row.GetCell(9).ToString().ToBoolean();
                    model.Listable = _listable;
                    var entity = new ProductModel();
                    var resultVersion = serverHelper.RequestGetReuse(VersionByIdUrl + model.Id);
                    var requstParm = JsonConvert.DeserializeObject<Root>(resultVersion);
                    if (requstParm != null)
                    {
                        entity.Id = model.Id;
                        entity.ProductId = requstParm.Data.ProductId;
                        entity.Name = model.Name;
                        entity.CategoryId = model.CategoryId;
                        entity.Style = requstParm.Data.Style;
                        entity.Components = requstParm.Data.Components;
                        entity.Size = requstParm.Data.Size;
                        entity.FunctionType = requstParm.Data.FunctionType;
                        entity.IsSharedModel = requstParm.Data.IsSharedModel;
                        entity.IsVirtualProduct = requstParm.Data.IsVirtualProduct;
                        entity.OwnerId = model.OwnerId;
                        entity.Images = model.Images;
                        entity.Description = model.Description;
                        entity.CustomAttributes = model.CustomAttributes;
                        entity.CreatedUtc = requstParm.Data.CreatedUtc;
                        entity.UpdatedUtc = string.Format("{0:yyyy-MM-dd}t{0:hh:mm:ss}z", DateTime.Now).ToString().ToUpper();
                        entity.Latest = requstParm.Data.Latest;
                        entity.Revision = requstParm.Data.Revision;
                        entity.IsLiked = requstParm.Data.IsLiked;
                        entity.LikedNumber = requstParm.Data.LikedNumber;
                        entity.IsCollected = requstParm.Data.IsCollected;
                        entity.CollectedNumber = requstParm.Data.CollectedNumber;
                        entity.ExtraData = requstParm.Data.ExtraData;
                        entity.Published = model.Published;
                        entity.BoundId = requstParm.Data.BoundId;
                        entity.Tags = requstParm.Data.Tags;
                        entity.TagIds = requstParm.Data.TagIds;
                        entity.Listable = model.Listable;
                        _ConcurrenProducts.Enqueue(entity);
                    }

                }
                int Count = 0;
                ProductModel queueElement = null;
                while (_ConcurrenProducts.Count > 0)
                {
                    Count++;

                    bool gotElement = _ConcurrenProducts.TryDequeue(out queueElement);

                    if (queueElement != null)
                    {
                        var entityJson = JsonConvert.SerializeObject(queueElement);
                        string result = updatePublishEntity(queueElement, entityJson, token);
                        var falg = result.Contains("errorCode");
                        if (falg)
                        {
                            strList.Add(queueElement.Id + "更新发布失败." + result);
                        }
                        else
                        {
                            strList.Add(queueElement.Id + "更新发布成功！" + result + "/");
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                return Json(new { result = true, message = strList, totaltime = ts2.TotalSeconds, CountInfo = Count, swCount = stCount }, "text/html");

            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message }, "text/html");
            }
        }

        private string updatePublishEntity(ProductModel model, string entityJson, string token)
        {
            return serverHelper.RequestPutReuse(updateAndPublishUrl + "/" + model.Id, entityJson, token);
        }

    }

}