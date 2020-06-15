using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Toyota.Models.Dto;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Toyota.Models
{
    public class ClassCrud
    {
        private static string strConn = Ut.GetMySQLConnect();
        public static List<CarTypeInfo> GetListCarTypeInfo(string vin)
        {
                List<CarTypeInfo> list = null;

                vin = vin.Substring(0, 8);
                try
                {
                    #region strCommand
                    string strCommand = "  SELECT " +
                                        "  CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) AS id, " +
                                        "  m.model_name, " +
                                        "  mc.model_code, " +
                                        "  mc.engine1, " +
                                        "   mc.body, " +
                                        "   mc.grade, " +
                                        "   mc.trans, " +
                                        "   mc.frame, " +
                                        "   mc.sysopt, " +
                                        "   mc.f1, " +
                                        "   mc.f2, " +
                                        "   mc.f3 " +
                                        "   FROM " +
                                        "   model_codes mc " +
                                        "   LEFT JOIN models m ON mc.catalog = m.catalog " +
                                        "   AND mc.catalog_code = m.catalog_code " +
                                        "   WHERE " +
                                        "   mc.vin8 = @vin;  ";
                #endregion

                     using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        list = db.Query<CarTypeInfo>(strCommand, new { vin }).ToList();
                    }
                }
                catch(Exception ex)
                {
                    string Error = ex.Message;
                    int o = 0;
                }

            return list;
        }
        public static List<ModelCar> GetModelCars()
        {
            List<ModelCar> list = null;

            #region MyRegion
            string strCommand = " SELECT DISTINCT REPLACE(m.model_name, ' ', '') model_id, " +
                                " m.model_name model, " +
                                " REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(m.model_name, '/', ''), '(', ''), ')', ''), '#', ''),'     '," +
                                "' '),',','-'),'  ',' '), ' ', '-'), '.', ''), '---', '-'),'--', '-') seo_url " +
                                " FROM " +
                                " models m ";
            #endregion
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string vehicle_id, string code_lang = "EN")
        {
            List<PartsGroup> list = null;
            string strCommand = "   SELECT " +
                                "   mgroup_num id, " +
                                "   group_name name " +
                                "   FROM " +
                                "   mgroup_name; ";

                        try
                        {
                            using (IDbConnection db = new MySqlConnection(strConn))
                            {
                                list = db.Query<PartsGroup>(strCommand).ToList();
                            }

                            for(int i =0;i<list.Count; i++)
                            {
                                 List<Sgroups> listSgroups = GetSgroups(vehicle_id, list[i].Id, code_lang );
                                 list[i].childs = listSgroups;
                            }

                        }
                        catch(Exception ex)
                        {
                            string Error = ex.Message;
                            int o = 0;
                        }
                        return list;
        }
        public static List<header> GetHeaders()
        {
            List<header> list = new List<header>();

            header header1 = new header { code  = "id", title = "ИД" };
            list.Add(header1);
            header header2 = new header { code  = "model_name", title = "Модель" };
            list.Add(header2);
            header header3 = new header { code  = "model_code", title = "Код модели" };
            list.Add(header3);
            header header4 = new header { code  = "engine1", title = "Двигатель" };
            list.Add(header4);
            header header5 = new header { code  = "body", title = "Кузов" };
            list.Add(header5);
            header header6 = new header { code  = "grade", title = "Класс" };
            list.Add(header6);
            header header7 = new header { code  = "trans", title = "Трансмиссия" };
            list.Add(header7);
            header header8 = new header { code  = "frame", title = "Серия" };
            list.Add(header8);
            header header9 = new header { code  = "sysopt", title = "Сист опц" };
            list.Add(header9);
            header header10 = new header { code  = "f1", title = "f1" };
            list.Add(header10);
            header header11 = new header { code  = "f2", title = "f2" };
            list.Add(header11);
            header header12 = new header { code  = "f3", title = "f3" };
            list.Add(header12);

            return list;
        }
        public static List<SpareParts> GetSpareParts(string group_id, string code_lang = "EU")
        {
            List<SpareParts> list = new List<SpareParts>();

            string[] pstrArr = group_id.Split("_");

            string catalog = pstrArr[0];
            string catalog_code = pstrArr[1];
            string part_group = pstrArr[2];

            try
            {
                #region strCommand

                string strCommand = " SELECT CONCAT(pc.catalog, '_', pc.catalog_code, '_', pc.part_group, '_', t1.pnc) id, " +
                                    " t1.name, " +
                                    " pd.desc_en deskr, " +
                                    " pp.pic_code image_id " +
                                    " FROM part_codes pc " +
                                    " LEFT JOIN " +
                                    " (SELECT p.pnc, p.desc_lang name  FROM " +
                                    " pncs p " +
                                    " WHERE " +
                                    " p.catalog = @catalog " +
                                    " AND p.pnc IN " +
                                    " (SELECT pc.pnc FROM " +
                                    " part_codes pc " +
                                    " WHERE pc.catalog  = @catalog " +
                                    " AND p.code_lang = @code_lang " + 
                                    " AND pc.catalog_code = @catalog_code " +
                                    " AND pc.part_group = @part_group ))t1 " +
                                    " ON pc.pnc = t1.pnc " +
                                    " LEFT JOIN  pg_pictures pp ON pc.catalog = pp.catalog " +
                                    " AND pc.catalog_code = pp.catalog_code " +
                                    " AND pc.part_group = pp.part_group " +
                                    " LEFT JOIN pic_desc pd ON pc.catalog = pd.catalog " +
                                    " AND pc.catalog_code = pd.catalog_code " +
                                    " AND pp.pic_desc_code = pd.pic_num " +

                                    " WHERE pc.catalog = @catalog " +
                                    " AND pc.catalog_code = @catalog_code " +
                                    " AND pc.part_group = @part_group ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<SpareParts>(strCommand, new { catalog, catalog_code, part_group, code_lang }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                     list[i].hotspots = GetHotspots(group_id, code_lang);
                }
            }

            return list;
        }
        public static List<Filters> GetFilters(string vin8)
        {
            vin8 = vin8.Substring(0, 8);
            List<Filters> filters = new List<Filters>();

            try
            {
                #region Модель
                List<string> modelList = new List<string>();

                   string modelCom = " SELECT DISTINCT m.model_name " +
                                    " FROM model_codes mc " +
                                    " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                    " AND mc.catalog_code = m.catalog_code " +
                                    " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    modelList = db.Query<string>(modelCom, new { vin8 }).ToList();
                }
                Filters modelF = new Filters { Id = "1", name = "Модель" };
                List<values> modelVal = new List<values>();

                for (int i = 0; i < modelList.Count; i++)
                {
                    values v1 = new values { Id = modelList[i], name = modelList[i] };
                    modelVal.Add(v1);
                }

                modelF.values = modelVal;
                filters.Add(modelF);
                #endregion

                #region Код модели
                List<string> codeList = new List<string>();

                string codeCom = " SELECT DISTINCT mc.model_code " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    codeList = db.Query<string>(codeCom, new { vin8 }).ToList();
                }
                Filters codeF = new Filters { Id = "2", name = "Код модели" };
                List<values> codeVal = new List<values>();

                for (int i = 0; i < codeList.Count; i++)
                {
                    values v1 = new values { Id = codeList[i], name = codeList[i] };
                    codeVal.Add(v1);
                }

                codeF.values = codeVal;
                filters.Add(codeF);
                #endregion

                #region Двигатель
                List<string> engList = new List<string>();

                string engCom = " SELECT DISTINCT mc.engine1 " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    engList = db.Query<string>(engCom, new { vin8 }).ToList();
                }
                Filters engF = new Filters { Id = "3", name = "Двигатель" };
                List<values> engVal = new List<values>();

                for (int i = 0; i < engList.Count; i++)
                {
                    values v1 = new values { Id = engList[i], name = engList[i] };
                    engVal.Add(v1);
                }

                engF.values = engVal;
                filters.Add(engF);
                #endregion

                #region Кузов
                List<string> bodyList = new List<string>();

                string bodyCom = " SELECT DISTINCT mc.body " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    bodyList = db.Query<string>(bodyCom, new { vin8 }).ToList();
                }
                Filters bodyF = new Filters { Id = "4", name = "Кузов" };
                List<values> bodyVal = new List<values>();

                for (int i = 0; i < bodyList.Count; i++)
                {
                    values v1 = new values { Id = bodyList[i], name = bodyList[i] };
                    bodyVal.Add(v1);
                }

                bodyF.values = bodyVal;
                filters.Add(bodyF);
                #endregion

                #region Класс
                List<string> gradeList = new List<string>();

                string gradeCom = " SELECT DISTINCT mc.grade " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    gradeList = db.Query<string>(gradeCom, new { vin8 }).ToList();
                }
                Filters gradeF = new Filters { Id = "5", name = "Класс" };
                List<values> gradeVal = new List<values>();

                for (int i = 0; i < gradeList.Count; i++)
                {
                    values v1 = new values { Id = gradeList[i], name = gradeList[i] };
                    gradeVal.Add(v1);
                }

                gradeF.values = gradeVal;
                filters.Add(gradeF);
                #endregion

                #region Трансмиссия
                List<string> transList = new List<string>();

                string transCom = " SELECT DISTINCT mc.trans " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    transList = db.Query<string>(transCom, new { vin8 }).ToList();
                }
                Filters transF = new Filters { Id = "6", name = "Трансмиссия" };
                List<values> transVal = new List<values>();

                for (int i = 0; i < transList.Count; i++)
                {
                    values v1 = new values { Id = transList[i], name = transList[i] };
                    transVal.Add(v1);
                }

                transF.values = transVal;
                filters.Add(transF);
                #endregion

                #region Серия
                List<string> frameList = new List<string>();

                string frameCom = " SELECT DISTINCT mc.frame " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    frameList = db.Query<string>(frameCom, new { vin8 }).ToList();
                }
                Filters frameF = new Filters { Id = "7", name = "Серия" };
                List<values> frameVal = new List<values>();

                for (int i = 0; i < frameList.Count; i++)
                {
                    values v1 = new values { Id = frameList[i], name = frameList[i] };
                    frameVal.Add(v1);
                }

                frameF.values = frameVal;
                filters.Add(frameF);
                #endregion

                #region Сист.опц
                List<string> sysoptList = new List<string>();

                string sysoptCom = " SELECT DISTINCT mc.sysopt " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    sysoptList = db.Query<string>(sysoptCom, new { vin8 }).ToList();
                }
                Filters sysoptF = new Filters { Id = "8", name = "Сист.опц" };
                List<values> sysoptVal = new List<values>();

                for (int i = 0; i < sysoptList.Count; i++)
                {
                    values v1 = new values { Id = sysoptList[i], name = sysoptList[i] };
                    sysoptVal.Add(v1);
                }

                sysoptF.values = sysoptVal;
                filters.Add(sysoptF);
                #endregion

                #region f1
                List<string> f1List = new List<string>();

                string f1Com = " SELECT DISTINCT mc.f1 " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    f1List = db.Query<string>(f1Com, new { vin8 }).ToList();
                }
                Filters f1F = new Filters { Id = "9", name = "f1" };
                List<values> f1Val = new List<values>();

                for (int i = 0; i < f1List.Count; i++)
                {
                    values v1 = new values { Id = f1List[i], name = f1List[i] };
                    f1Val.Add(v1);
                }

                f1F.values = f1Val;
                filters.Add(f1F);
                #endregion

                #region f2
                List<string> f2List = new List<string>();

                string f2Com = " SELECT DISTINCT mc.f2 " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    f2List = db.Query<string>(f2Com, new { vin8 }).ToList();
                }
                Filters f2F = new Filters { Id = "10", name = "f2" };
                List<values> f2Val = new List<values>();

                for (int i = 0; i < f2List.Count; i++)
                {
                    values v1 = new values { Id = f2List[i], name = f2List[i] };
                    f2Val.Add(v1);
                }

                f2F.values = f2Val;
                filters.Add(f2F);
                #endregion

                #region f3
                List<string> f3List = new List<string>();

                string f3Com = " SELECT DISTINCT mc.f3 " +
                                 " FROM model_codes mc " +
                                 " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                 " AND mc.catalog_code = m.catalog_code " +
                                 " WHERE mc.vin8 = @vin8; ";

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    f3List = db.Query<string>(f3Com, new { vin8 }).ToList();
                }
                Filters f3F = new Filters { Id = "11", name = "f3" };
                List<values> f3Val = new List<values>();

                for (int i = 0; i < f3List.Count; i++)
                {
                    values v1 = new values { Id = f3List[i], name = f3List[i] };
                    f3Val.Add(v1);
                }

                f3F.values = f3Val;
                filters.Add(f3F);
                #endregion
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            return filters;
        }
        public static List<Sgroups> GetSgroups(string vehicle_id, string  group_id, string code_lang = "EN")
        {
            List<Sgroups> list = null;

            string[] strArr = vehicle_id.Split("_");

            string catalog = strArr[0];
            string catalog_code = strArr[1];

            try
            {
                #region strCommand
                string strCommand = " SELECT " +
                                    " CONCAT(php1.catalog, '_', php1.catalog_code, '_', php1.part_group) node_id, " +
                                    " t1.name, "+
                                    " php1.pic_code image_id,  " +
                                    " '.png' image_ext  " +
                                    " FROM pg_header_pics php1 " +
                                    " LEFT JOIN " +
                                    " (SELECT pg.group_id id, " +
                                    " pg.desc_lang name " +
                                    " FROM part_groups pg " +
                                    " WHERE pg.catalog = @catalog " +
                                    " AND pg.code_lang = @code_lang  " +
                                    " AND pg.group_id IN " +
                                    " (SELECT php.part_group FROM " +
                                    " pg_header_pics php " +
                                    " WHERE php.catalog = @catalog " +
                                    " AND php.catalog_code = @catalog_code ))t1 " +
                                    " ON php1.part_group = t1.id " +
                                    " WHERE php1.catalog = @catalog " +
                                    " AND php1.main_group = @main_group " +
                                    " AND php1.catalog_code = @catalog_code; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<Sgroups>(strCommand, new { catalog, catalog_code, code_lang, main_group = group_id }).ToList();
                }
            }
            catch(Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return list;
        }
        public static List<CarTypeInfo> GetListCarTypeInfoFilterCars(string vin8, string [] param)
        {
                List<CarTypeInfo> list = null;

                for(int i =0; i< param.Length; i++)
                {
                    if(param[i] == null)
                    {
                        param[i] = "";
                    }
                }

                vin8 = vin8.Substring(0, 8);

               string model_name = param[0];
               string model_code = param[1];
               string engine1 = param[2];
               string body = param[3];
               string grade = param[4];
               string trans = param[5];
               string frame = param[6];
               string sysopt = param[7];
               string f1 = param[8];
               string f2 = param[9];
               string f3 = param[10];

            try
            {
                #region strCommand
                string strCommand = " SELECT " +
                                    "  CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) AS id, " +
                                    "  m.model_name, " +
                                    "   mc.model_code, " +
                                    "   mc.engine1, " +
                                    "   mc.body, " +
                                    "   mc.grade, " +
                                    "   mc.trans, " +
                                    "   mc.frame, " +
                                    "   mc.sysopt, " +
                                    "   mc.f1, " +
                                    "   mc.f2, " +
                                    "   mc.f3 " +
                                    "   FROM " +
                                    "   model_codes mc " +
                                    "   LEFT JOIN models m ON mc.catalog = m.catalog " +
                                    "   AND mc.catalog_code = m.catalog_code " +
                                    "   WHERE " +
                                    "   mc.vin8 = @vin8 " +
                                    "   AND m.model_name = @model_name " +
                                    "   AND mc.model_code = @model_code " +
                                    "   AND mc.engine1 = @engine1 " +
                                    "   AND mc.body = @body " +
                                    "   AND mc.grade = @grade " +
                                    "   AND mc.trans = @trans " +
                                    "   AND mc.frame = @frame " +
                                    "   AND mc.sysopt = @sysopt " +
                                    "   AND mc.f1 = @f1 " +
                                    "   AND mc.f2 = @f2 " +
                                    "   AND mc.f3 = @f3 ;";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        list = db.Query<CarTypeInfo>(strCommand, new { vin8, model_name, model_code, engine1, body, grade, trans, frame, sysopt, f1, f2, f3 }).ToList();
                    }
                }
                catch(Exception ex)
                {
                    string Errror = ex.Message;
                    int y = 0;
                }
            
            return list;
        }
        public static List<lang> GetLang()
        {
            List<lang> list = new List<lang>();
            string strCommand = "   SELECT code, name, is_default FROM lang; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<lang>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<string> GetWmi()
        {
            List<string> list = new List<string>();
            string strCommand = " SELECT value FROM wmi; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<string>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<attributes> GetAttributes()
        {
            List<attributes> list = new List<attributes>();

            attributes cmodnamepc = new attributes { code = "model_name", name = "Модель автомобиля", value = "" };
            list.Add(cmodnamepc);

            attributes xcardrs = new attributes { code = "model_code", name = "Код модели", value = "" };
            list.Add(xcardrs);

            attributes dmodyr = new attributes { code = "engine1", name = "Двигатель", value = "" };
            list.Add(dmodyr);

            attributes xgradefulnam = new attributes { code = "body", name = "Кузов", value = "" };
            list.Add(xgradefulnam);

            attributes ctrsmtyp = new attributes { code = "grade", name = "Класс", value = "" };
            list.Add(ctrsmtyp);

            attributes cmftrepc = new attributes { code = "trans", name = "Трансмиссия", value = "" };
            list.Add(cmftrepc);

            attributes carea = new attributes { code = "frame", name = "Серия", value = "" };
            list.Add(carea);

            attributes sysopt = new attributes { code = "sysopt", name = "Сист.Опц.", value = "" };
            list.Add(sysopt);

            attributes f1 = new attributes { code = "f1", name = "f1", value = "" };
            list.Add(f1);

            attributes f2 = new attributes { code = "f2", name = "f2", value = "" };
            list.Add(f2);

            attributes f3 = new attributes { code = "f3", name = "f3", value = "" };
            list.Add(f3);

            return list;
        }
        public static VehiclePropArr GetVehiclePropArr(string vehicle_id)
        {
            VehiclePropArr model = null;

            try
            {
                string[] strArr = vehicle_id.Split("_");

                if (strArr.Length == 4)
                {
                    string catalog = strArr[0];
                    string catalog_code = strArr[1];
                    string compl_code = strArr[2];
                    string sysopt = strArr[3];

                    #region strCommand
                    string strCommand = " SELECT " +
                                        " CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) AS id, " +
                                        " m.model_name,   " +
                                        " mc.model_code,   " +
                                        " mc.engine1,  " +
                                        " mc.body,  " +
                                        " mc.grade,  " +
                                        " mc.trans, " +
                                        " mc.frame,  " +
                                        " mc.sysopt, " +
                                        " mc.f1,  " +
                                        " mc.f2, " +
                                        " mc.f3 " +
                                        " FROM " +
                                        " model_codes mc " +
                                        " LEFT JOIN models m ON mc.catalog = m.catalog " +
                                        " AND mc.catalog_code = m.catalog_code " +
                                        " WHERE " +
                                        " mc.catalog = @catalog " +
                                        " AND mc.catalog_code = @catalog_code " +
                                        " AND mc.compl_code = @compl_code " +
                                        " AND mc.sysopt = @sysopt  LIMIT 1; ";
                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        CarTypeInfo carType = db.Query<CarTypeInfo>(strCommand, new { catalog, catalog_code, compl_code, sysopt }).FirstOrDefault();

                        List<attributes> list = GetAttributes();

                        list[0].value = carType.model_name;
                        list[1].value = carType.model_code;
                        list[2].value = carType.engine1;
                        list[3].value = carType.body;
                        list[4].value = carType.grade;
                        list[5].value = carType.trans;
                        list[6].value = carType.frame;
                        list[7].value = carType.sysopt;
                        list[8].value = carType.f1;
                        list[9].value = carType.f2;
                        list[10].value = carType.f3;

                        model = new VehiclePropArr { model_name = carType.model_name };
                        model.attributes = list;
                    }
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return model;
        }
        public static List<node> GetNodes(string [] codesArr =null, string[]  node_idsArr  = null)
        {
            List<node> list = new List<node>();
            string codes = null;
            string node_ids = null;

            if (codesArr != null && codesArr.Length > 0)
            {
                codes = string.Empty;

                for (int i=0; i< codesArr.Length; i++)
                {
                    if(i == 0)
                    {
                        codes += codesArr[i];
                    }
                    else
                    {
                        codes += "," + codesArr[i];
                    }
                    
                }
            }


            if (node_idsArr != null && node_idsArr.Length > 0)
            {
                node_ids = string.Empty;

                for (int i = 0; i < node_idsArr.Length; i++)
                {
                    if (i == 0)
                    {
                        node_ids += node_idsArr[i];
                    }
                    else
                    {
                        node_ids += "," + node_idsArr[i];
                    }
                }
            }

            string strCommand = " SELECT nt.code, nt.group name, nt.node_ids FROM " +
                                " nodes_tb nt ";

            if(!String.IsNullOrEmpty(codes) || !String.IsNullOrEmpty(node_ids ))
            {
                strCommand += " WHERE";
            }

            if(!String.IsNullOrEmpty( codes ))
            {
                strCommand += $"  nt.code IN  ({codes}) ";
            }

            if (!String.IsNullOrEmpty(codes) && !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " OR ";
            }

            if (!String.IsNullOrEmpty(node_ids))
            {
                strCommand += $"  nt.node_ids IN  ({node_ids}) ";
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<node>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static DetailsInNode GetDetailsInNode(string node_id, string lang = "EN")
        {
            string [] strArr = node_id.Split("_");

            string catalog = strArr[0];
            string catalog_code = strArr[1];
            string group_id = strArr[2];

            DetailsInNode detailsInNode = new DetailsInNode { node_id = node_id };

            string strCommand = " SELECT desc_lang " +
                                " FROM part_groups " +
                                " WHERE catalog = @catalog " +
                                " AND group_id = @group_id " +
                                " AND code_lang = @lang " +
                                " LIMIT 1; ";

            string strCommDeatil = "  SELECT CONCAT(p.catalog, '_', p.pnc, '_', p.code_lang) number,  " +
            " p.desc_lang name " +
            " FROM " +
            " pncs p " +
            " WHERE " +
            " p.pnc IN " +
            " (SELECT DISTINCT pc.pnc " +
            " FROM part_codes pc " +
            " WHERE " +
            " pc.catalog = @catalog " +
            " AND pc.catalog_code = @catalog_code " +
            " AND pc.part_group = @group_id) " +
            " AND p.catalog = @catalog " +
            " AND p.code_lang = @lang; ";


             string strCommImages = " SELECT  " +
                        " pc.pic_code id, " +
                        " pc.img_format ext " +
                        " FROM " +
                        " pg_pictures pc " +
                        " WHERE " +
                        " pc.catalog = @catalog  " +
                        " AND pc.catalog_code = @catalog_code " +
                        " AND pc.part_group = @group_id ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    detailsInNode.name = db.Query<string>(strCommand, new { catalog, group_id, lang }).FirstOrDefault();
                    detailsInNode.parts = db.Query<Detail>(strCommDeatil, new { catalog, catalog_code, group_id, lang }).ToList();
                    detailsInNode.images = db.Query<images>(strCommImages, new { catalog, catalog_code, group_id }).ToList();
                }

                List<hotspots> hotspots = GetHotspots(node_id, lang);

                for (int i = 0; i < detailsInNode.parts.Count; i++)
                {
                    detailsInNode.parts[i].hotspots = hotspots;
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int y = 0;
            }

            return detailsInNode;
        }
        public static List<hotspots> GetHotspots(string node_id, string lang = "EN")
        {
            string[] strParam = node_id.Split("_");

            string catalog = strParam[0];
            string catalog_code = strParam[1];
            string part_group = strParam[2];


            if (!String.IsNullOrEmpty(lang))
            {
                strConn = Ut.GetMySQLConnect(lang);
            }

            List<hotspots> list = null;
            try
            {
                #region strCommand
                string strCommand = " SELECT " +
                                    " CONCAT(catalog, '_', pic_code, '_', label2) hotspot_id, " +  
                                    " CONCAT(pic_code, img_format) image_id, " +
                                    "   x1, " +
                                    "   y1, " +
                                    "   x2, " +
                                    "   y2 " +
                                    " FROM images " +
                                    " WHERE  " +
                                    " catalog = @catalog AND " +
                                    " pic_code IN " +
                                    " (SELECT php1.pic_code " +
                                    " FROM pg_header_pics php1 " +
                                    " WHERE php1.catalog = @catalog " +
                                    " AND php1.catalog_code = @catalog_code " +
                                    " AND php1.part_group = @part_group ); ";

                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<hotspots>(strCommand, new { catalog, catalog_code, part_group }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            return list;
        }
    }
}
