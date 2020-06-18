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
                                        "  CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) vehicle_id, " +
                                        "  mc.model_name, " +
                                        "  mc.brand, " +
                                        "  mc.catalog, " +
                                        "  mc.model_code, " +
                                        "  mc.add_codes, " +
                                        "  mc.engine1 engine, " +
                                        "  mc.prod_start, " +
                                        "  mc.grade, " +
                                        "  mc.atm_mtm, " +
                                        "  mc.trans, " +
                                        "  mc.f1 " +
                                        "  FROM " +
                                        "  model_codes mc " +
                                        "  WHERE mc.vin8 = @vin;  ";

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
        public static List<ModelCar> GetModelCars(string brand_id = "TOYOTA")
        {
            List<ModelCar> list = null;

            #region MyRegion
            string strCommand = " SELECT DISTINCT mc.model_id, mc.model_name model, " +
                                " REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(mc.model_name, '/', ''), '(', ''), ')', ''), '#', ''),'     '," +
                                "' '),',','-'),'  ',' '), ' ', '-'), '.', ''), '---', '-'),'--', '-') seo_url " +
                                " FROM model_codes mc " +
                                " WHERE mc.brand = @brand_id ";
            #endregion
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand, new { brand_id }).ToList();
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
                                "   mgroup_num group_id, " +
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
                                 List<Sgroups> listSgroups = GetSgroups(vehicle_id, list[i].group_id, code_lang );
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

            header vehicle_id = new header { code  = "vehicle_id", title = "ИД" };
            list.Add(vehicle_id);
            header model_name = new header { code  = "model_name", title = "Модель" };
            list.Add(model_name);
            header brand = new header { code = "brand", title = "Бренд" };
            list.Add(brand);
            header catalog = new header { code = "catalog", title = "Каталог" };
            list.Add(catalog);
            header model_code = new header { code  = "model_code", title = "Код модели" };
            list.Add(model_code);
            header add_codes = new header { code = "add_codes", title = "Доп коды" };
            list.Add(add_codes);
            header engine = new header { code  = "engine", title = "Двигатель" };
            list.Add(engine);
            header prod_start = new header { code = "prod_start", title = "Год выпуска" };
            list.Add(prod_start);
            header grade = new header { code = "grade", title = "Класс" };
            list.Add(grade);
            header atm_mtm = new header { code  = "atm_mtm", title = "Коробка передач" };
            list.Add(atm_mtm);
            header trans = new header { code  = "trans", title = "Трансмиссия" };
            list.Add(trans);
            header f1 = new header { code  = "f1", title = "f1" };
            list.Add(f1);
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
        public static List<Filters> GetFilters(string model_id, string[] param, string brand_id = "TOYOTA")
        {
            List<Filters> filters = new List<Filters>();
            try
            {
                

                #region string Where

                string catalogWhereIN = "";
                string model_codeWhereIN = "";
                string add_codesWhereIN = "";
                string prod_startWhereIN = "";
                string engineWhereIN = "";
                string gradeWhereIN = "";
                string atm_mtmWhereIN = "";
                string transWhereIN = "";
                string f1WhereIN = "";

                if (param.Length > 0)
                {
                    for(int i=0; i<param.Length; i++)
                    {
                        if (param[i].IndexOf("catalog") > -1)
                        {
                            if(String.IsNullOrEmpty(catalogWhereIN))
                            {
                                catalogWhereIN = " '" + param[i].Substring(param[i].IndexOf("catalog_") + "catalog_".Length, param[i].Length - (param[i].IndexOf("catalog_") + "catalog_".Length)) + "' ";
                            }
                            else
                            {
                                catalogWhereIN += ", '" + param[i].Substring(param[i].IndexOf("catalog_") + "catalog_".Length, param[i].Length - (param[i].IndexOf("catalog_") + "catalog_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("model_code") > -1)
                        {
                            if (String.IsNullOrEmpty(model_codeWhereIN))
                            {
                                model_codeWhereIN = " '" + param[i].Substring(param[i].IndexOf("model_code_") + "model_code_".Length, param[i].Length - (param[i].IndexOf("model_code_") + "model_code_".Length)) + "' ";
                            }
                            else
                            {
                                model_codeWhereIN += ", '" + param[i].Substring(param[i].IndexOf("model_code_") + "model_code_".Length, param[i].Length - (param[i].IndexOf("model_code_") + "model_code_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("add_codes") > -1)
                        {
                            if (String.IsNullOrEmpty(add_codesWhereIN))
                            {
                                add_codesWhereIN = " '" + param[i].Substring(param[i].IndexOf("add_codes_") + "add_codes_".Length, param[i].Length - (param[i].IndexOf("add_codes_") + "add_codes_".Length)) + "' ";
                            }
                            else
                            {
                                add_codesWhereIN += ", '" + param[i].Substring(param[i].IndexOf("add_codes_") + "add_codes_".Length, param[i].Length - (param[i].IndexOf("add_codes_") + "add_codes_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("engine") > -1)
                        {
                            if (String.IsNullOrEmpty(engineWhereIN))
                            {
                                engineWhereIN = " '" + param[i].Substring(param[i].IndexOf("engine_") + "engine_".Length, param[i].Length - (param[i].IndexOf("engine_") + "engine_".Length)) + "' ";
                            }
                            else
                            {
                                engineWhereIN += ", '" + param[i].Substring(param[i].IndexOf("engine_") + "engine_".Length, param[i].Length - (param[i].IndexOf("engine_") + "engine_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("prod_start") > -1)
                        {
                            if (String.IsNullOrEmpty(prod_startWhereIN))
                            {
                                prod_startWhereIN = " '" + param[i].Substring(param[i].IndexOf("prod_start_") + "prod_start_".Length, param[i].Length - (param[i].IndexOf("prod_start_") + "prod_start_".Length)) + "' ";
                            }
                            else
                            {
                                prod_startWhereIN += ", '" + param[i].Substring(param[i].IndexOf("prod_start_") + "prod_start_".Length, param[i].Length - (param[i].IndexOf("prod_start_") + "prod_start_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("grade") > -1)
                        {
                            if (String.IsNullOrEmpty(gradeWhereIN))
                            {
                                gradeWhereIN = " '" + param[i].Substring(param[i].IndexOf("grade_") + "grade_".Length, param[i].Length - (param[i].IndexOf("grade_") + "grade_".Length)) + "' ";
                            }
                            else
                            {
                                gradeWhereIN += ", '" + param[i].Substring(param[i].IndexOf("grade_") + "grade_".Length, param[i].Length - (param[i].IndexOf("grade_") + "grade_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("atm_mtm") > -1)
                        {
                            if (String.IsNullOrEmpty(atm_mtmWhereIN))
                            {
                                atm_mtmWhereIN = " '" + param[i].Substring(param[i].IndexOf("atm_mtm_") + "atm_mtm_".Length, param[i].Length - (param[i].IndexOf("atm_mtm_") + "atm_mtm_".Length)) + "' ";
                            }
                            else
                            {
                                atm_mtmWhereIN += ", '" + param[i].Substring(param[i].IndexOf("atm_mtm_") + "atm_mtm_".Length, param[i].Length - (param[i].IndexOf("atm_mtm_") + "atm_mtm_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("trans") > -1)
                        {
                            if (String.IsNullOrEmpty(transWhereIN))
                            {
                                transWhereIN = " '" + param[i].Substring(param[i].IndexOf("trans_") + "trans_".Length, param[i].Length - (param[i].IndexOf("trans_") + "trans_".Length)) + "' ";
                            }
                            else
                            {
                                transWhereIN += ", '" + param[i].Substring(param[i].IndexOf("trans_") + "trans_".Length, param[i].Length - (param[i].IndexOf("trans_") + "trans_".Length)) + "' ";
                            }
                        }
                        else if (param[i].IndexOf("f1") > -1)
                        {
                            if (String.IsNullOrEmpty(f1WhereIN))
                            {
                                f1WhereIN = " '" + param[i].Substring(param[i].IndexOf("f1_") + "f1_".Length, param[i].Length - (param[i].IndexOf("f1_") + "f1_".Length)) + "' ";
                            }
                            else
                            {
                                f1WhereIN += ", '" + param[i].Substring(param[i].IndexOf("f1_") + "f1_".Length, param[i].Length - (param[i].IndexOf("f1_") + "f1_".Length)) + "' ";
                            }
                        }
                    }
                }
                #endregion

                #region Модель

                List<string> model_nameList = new List<string>();
                string model_nameCom = " SELECT DISTINCT " +
                                       " mc.model_name " +
                                       " FROM model_codes mc " +
                                       " WHERE mc.model_id = @model_id " +
                                       " AND mc.brand = @brand_id ";

                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    model_nameCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    model_nameCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    model_nameCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    model_nameCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    model_nameCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    model_nameCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    model_nameCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    model_nameCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    model_nameCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    model_nameList = db.Query<string>(model_nameCom, new { model_id, brand_id }).ToList();
                }

                Filters modelF = new Filters { ﬁlter_id = "model_name", code = "model_name", name = "Модель" };
                List<values> modelVal = new List<values>();

                for (int i = 0; i < model_nameList.Count; i++)
                {
                    values model_v1 = new values { ﬁlter_item_id = "model_name_" + model_nameList[i], name = model_nameList[i] };
                    modelVal.Add(model_v1);
                }

                modelF.values = modelVal;
                filters.Add(modelF);

                #endregion

                #region Каталог

                List<string> catalogList = new List<string>(); //   catalog

                string catalogCom = " SELECT DISTINCT mc.catalog " +
                                    " FROM model_codes mc " +
                                    " WHERE mc.model_id = @model_id " +
                                    " AND mc.brand = @brand_id ";

                if(!String.IsNullOrEmpty(catalogWhereIN))
                {
                    catalogCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    catalogCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    catalogCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    catalogCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    catalogCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    catalogCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    catalogCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    catalogCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    catalogCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    catalogList = db.Query<string>(catalogCom, new { model_id, brand_id }).ToList();
                }
                Filters catalogF = new Filters { ﬁlter_id = "catalog" , code = "catalog", name = "Регион" };
                List<values> catalogVal = new List<values>();

                for (int i = 0; i < catalogList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "catalog_" + catalogList[i], name = catalogList[i] };
                    catalogVal.Add(v1);
                }

                catalogF.values = catalogVal;
                filters.Add(catalogF);

                #endregion

                #region model_code

                List<string> model_codeList = new List<string>(); //   model_code

                string model_codeCom = " SELECT DISTINCT mc.model_code " +
                                       " FROM model_codes mc " +
                                       " WHERE mc.model_id = @model_id " +
                                       " AND mc.brand = @brand_id ";

                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    model_codeCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    model_codeCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    model_codeCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    model_codeCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    model_codeCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    model_codeCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    model_codeCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    model_codeCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    model_codeCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    model_codeList = db.Query<string>(model_codeCom, new { model_id, brand_id }).ToList();
                }
                Filters model_codeF = new Filters { ﬁlter_id = "model_code", code = "model_code", name = "Код модели" };
                List<values> model_codeVal = new List<values>();

                for (int i = 0; i < model_codeList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "model_code_" + model_codeList[i], name = model_codeList[i] };
                    model_codeVal.Add(v1);
                }

                model_codeF.values = model_codeVal;
                filters.Add(model_codeF);

                #endregion

                #region add_codes

                List<string> add_codesList = new List<string>(); //   add_codes

                string add_codesCom = " SELECT DISTINCT mc.add_codes " +
                                       " FROM model_codes mc " +
                                       " WHERE mc.model_id = @model_id " +
                                       " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    add_codesCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    add_codesCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    add_codesCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    add_codesCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    add_codesCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    add_codesCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    add_codesCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    add_codesCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    add_codesCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                } 
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    add_codesList = db.Query<string>(add_codesCom, new { model_id, brand_id }).ToList();
                }
                Filters add_codesF = new Filters { ﬁlter_id = "add_codes", code = "add_codes", name = "Комплектация" };
                List<values> add_codesVal = new List<values>();

                for (int i = 0; i < add_codesList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "add_codes_" + add_codesList[i], name = add_codesList[i] };
                    add_codesVal.Add(v1);
                }

                add_codesF.values = add_codesVal;
                filters.Add(add_codesF);

                #endregion

                #region prod_start

                List<string> prod_startList = new List<string>(); //   prod_start

                string prod_startCom = " SELECT DISTINCT mc.prod_start " +
                                       " FROM model_codes mc " +
                                       " WHERE mc.model_id = @model_id " +
                                       " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    prod_startCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    prod_startCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    prod_startCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    prod_startCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    prod_startCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    prod_startCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    prod_startCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    prod_startCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    prod_startCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                } 
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    prod_startList = db.Query<string>(prod_startCom, new { model_id, brand_id }).ToList();
                }
                Filters prod_startF = new Filters { ﬁlter_id = "prod_start", code = "prod_start", name = "Год выпуска" };
                List<values> prod_startVal = new List<values>();

                for (int i = 0; i < prod_startList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "prod_start_" + prod_startList[i], name = prod_startList[i] };
                    prod_startVal.Add(v1);
                }

                prod_startF.values = prod_startVal;
                filters.Add(prod_startF);

                #endregion

                #region engine

                List<string> engineList = new List<string>(); //   engine

                 string engineCom = " SELECT DISTINCT mc.engine1 " +
                                    " FROM model_codes mc " +
                                    " WHERE mc.model_id = @model_id " +
                                    " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    engineCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    engineCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    engineCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    engineCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    engineCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    engineCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    engineCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    engineCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    engineCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    engineList = db.Query<string>(engineCom, new { model_id, brand_id }).ToList();
                }
                Filters engineF = new Filters { ﬁlter_id = "engine", code = "engine", name = "Двигатель" };
                List<values> engineVal = new List<values>();

                for (int i = 0; i < engineList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "engine_" + engineList[i], name = engineList[i] };
                    engineVal.Add(v1);
                }

                engineF.values = engineVal;
                filters.Add(engineF);

                #endregion

                #region grade

                List<string> gradeList = new List<string>(); //   grade

                string gradeCom = " SELECT DISTINCT mc.grade " +
                                   " FROM model_codes mc " +
                                   " WHERE mc.model_id = @model_id " +
                                   " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    gradeCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    gradeCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    gradeCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    gradeCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    gradeCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    gradeCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    gradeCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    gradeCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    gradeCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    gradeList = db.Query<string>(gradeCom, new { model_id, brand_id }).ToList();
                }
                Filters gradeF = new Filters { ﬁlter_id = "grade", code = "grade", name = "Класс" };
                List<values> gradeVal = new List<values>();

                for (int i = 0; i < gradeList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "grade_" + gradeList[i], name = gradeList[i] };
                    gradeVal.Add(v1);
                }

                gradeF.values = gradeVal;
                filters.Add(gradeF);

                #endregion

                #region atm_mtm

                List<string> atm_mtmList = new List<string>(); //   atm_mtm

                string atm_mtmCom = " SELECT DISTINCT mc.atm_mtm " +
                                   " FROM model_codes mc " +
                                   " WHERE mc.model_id = @model_id " +
                                   " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    atm_mtmCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    atm_mtmCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    atm_mtmCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    atm_mtmCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    atm_mtmCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    atm_mtmCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    atm_mtmCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    atm_mtmCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    atm_mtmCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    atm_mtmList = db.Query<string>(atm_mtmCom, new { model_id, brand_id }).ToList();
                }
                Filters atm_mtmF = new Filters { ﬁlter_id = "atm_mtm", code = "atm_mtm", name = "Коробка передач" };
                List<values> atm_mtmVal = new List<values>();

                for (int i = 0; i < atm_mtmList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "atm_mtm_" + atm_mtmList[i], name = atm_mtmList[i] };
                    atm_mtmVal.Add(v1);
                }

                atm_mtmF.values = atm_mtmVal;
                filters.Add(atm_mtmF);

                #endregion

                #region trans

                List<string> transList = new List<string>(); //   trans

                string transCom = " SELECT DISTINCT mc.trans " +
                                   " FROM model_codes mc " +
                                   " WHERE mc.model_id = @model_id " +
                                   " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    transCom += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    transCom += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    transCom += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    transCom += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    transCom += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    transCom += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    transCom += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    transCom += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    transCom += $" AND mc.f1 IN ({f1WhereIN}) ";
                }
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    transList = db.Query<string>(transCom, new { model_id, brand_id }).ToList();
                }
                Filters transF = new Filters { ﬁlter_id = "trans", code = "trans", name = "Трансмиссия" };
                List<values> transVal = new List<values>();

                for (int i = 0; i < transList.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "trans_" + transList[i], name = transList[i] };
                    transVal.Add(v1);
                }

                transF.values = transVal;
                filters.Add(transF);

                #endregion

                #region f1

                List<string> f1List = new List<string>(); //   f1

                 string f1Com = " SELECT DISTINCT mc.f1 " +
                                " FROM model_codes mc " +
                                " WHERE mc.model_id = @model_id " +
                                " AND mc.brand = @brand_id ";

                #region WHERE
                if (!String.IsNullOrEmpty(catalogWhereIN))
                {
                    f1Com += $" AND mc.catalog IN ({catalogWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(model_codeWhereIN))
                {
                    f1Com += $" AND mc.model_code IN ({model_codeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(add_codesWhereIN))
                {
                    f1Com += $" AND mc.add_codes IN ({add_codesWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(prod_startWhereIN))
                {
                    f1Com += $" AND mc.prod_start IN ({prod_startWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(engineWhereIN))
                {
                    f1Com += $" AND mc.engine1 IN ({engineWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(gradeWhereIN))
                {
                    f1Com += $" AND mc.grade IN ({gradeWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(atm_mtmWhereIN))
                {
                    f1Com += $" AND mc.atm_mtm IN ({atm_mtmWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(transWhereIN))
                {
                    f1Com += $" AND mc.trans IN ({transWhereIN}) ";
                }
                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    f1Com += $" AND mc.f1 IN ({f1WhereIN}) ";
                }
                #endregion

                if (!String.IsNullOrEmpty(f1WhereIN))
                {
                    f1Com += $" AND mc.f1 IN ({f1WhereIN}) ";
                }

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    f1List = db.Query<string>(f1Com, new { model_id, brand_id }).ToList();
                }
                Filters f1F = new Filters { ﬁlter_id = "f1", code = "f1", name = "Руль" };
                List<values> f1Val = new List<values>();

                for (int i = 0; i < f1List.Count; i++)
                {
                    values v1 = new values { ﬁlter_item_id = "f1_" + f1List[i], name = f1List[i] };
                    f1Val.Add(v1);
                }

                f1F.values = f1Val;
                filters.Add(f1F);

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
        public static List<CarTypeInfo> GetListCarTypeInfoFilterCars(string model_id, string [] param, string brand_id = "TOYOTA")
        {
                List<CarTypeInfo> list = null;

                for(int i =0; i< param.Length; i++)
                {
                    if(param[i] == null)
                    {
                        param[i] = "";
                    }
                }

            try
            {
                #region strCommand
                string strCommand = "  SELECT " +
                                    "  CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) vehicle_id, " +
                                    "  mc.model_name, " +
                                    "  mc.brand, " +
                                    "  mc.catalog, " +
                                    "  mc.model_code, " +
                                    "  mc.add_codes, " +
                                    "  mc.engine1 engine, " +
                                    "  mc.prod_start, " +
                                    "  mc.grade, " +
                                    "  mc.atm_mtm, " +
                                    "  mc.trans, " +
                                    "  mc.f1 " +
                                    " FROM model_codes mc " +
                                    " WHERE mc.model_id = @model_id " +
                                    " AND mc.brand = @brand_id ";

                if(param.Length > 0)
                {
                    string strWhere = GetWhereFromFilter(param);
                    strCommand += strWhere;
                    int o = 0;
                }

                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<CarTypeInfo>(strCommand, new { model_id, brand_id }).ToList();
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

            attributes vehicle_id = new attributes { code = "vehicle_id", name = "ИД", value = "" };
            list.Add(vehicle_id);
            attributes model_name = new attributes { code = "model_name", name = "Модель автомобиля", value = "" };
            list.Add(model_name);
            attributes brand = new attributes { code = "brand", name = "Бренд", value = "" };
            list.Add(brand);
            attributes catalog = new attributes { code = "catalog", name = "Каталог", value = "" };
            list.Add(catalog);
            attributes model_code = new attributes { code = "model_code", name = "Код модели", value = "" };
            list.Add(model_code);
            attributes add_codes = new attributes { code = "add_codes", name = "Доп коды", value = "" };
            list.Add(add_codes);
            attributes engine = new attributes { code = "engine", name = "Двигатель", value = "" };
            list.Add(engine);
            attributes prod_start = new attributes { code = "prod_start", name = "Год выпуска", value = "" };
            list.Add(prod_start);
            attributes grade = new attributes { code = "grade", name = "Класс", value = "" };
            list.Add(grade);
            attributes atm_mtm = new attributes { code = "atm_mtm", name = "Коробка передач", value = "" };
            list.Add(atm_mtm);
            attributes trans = new attributes { code = "trans", name = "Трансмиссия", value = "" };
            list.Add(trans);
            attributes f1 = new attributes { code = "f1", name = "Руль", value = "" };
            list.Add(f1);

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
                    string strCommand = "  SELECT " +
                                        "  CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt) vehicle_id, " +
                                        "  mc.model_name, " +
                                        "  mc.brand, " +
                                        "  mc.catalog, " +
                                        "  mc.model_code, " +
                                        "  mc.add_codes, " +
                                        "  mc.engine1 engine, " +
                                        "  mc.prod_start, " +
                                        "  mc.grade, " +
                                        "  mc.atm_mtm, " +
                                        "  mc.trans, " +
                                        "  mc.f1 " +
                                        "  FROM " +
                                        "  model_codes mc " +
                                        "  WHERE " +
                                        " mc.catalog = @catalog " +
                                        " AND mc.catalog_code = @catalog_code " +
                                        " AND mc.compl_code = @compl_code " +
                                        " AND mc.sysopt = @sysopt  LIMIT 1; ";


                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        CarTypeInfo carType = db.Query<CarTypeInfo>(strCommand, new { catalog, catalog_code, compl_code, sysopt }).FirstOrDefault();

                        List<attributes> list = GetAttributes();

                        #region MyRegion
                        //vehicle_id 
                        //model_name
                        //brand
                        //catalog
                        //model_code
                        //add_codes
                        //engine
                        //prod_start
                        //grade
                        //atm_mtm
                        //trans
                        //f1 
                        #endregion

                        list[0].value = carType.vehicle_id;
                        list[1].value = carType.model_name;
                        list[2].value = carType.brand;
                        list[3].value = carType.catalog;
                        list[4].value = carType.model_code;
                        list[5].value = carType.add_codes;
                        list[6].value = carType.engine;
                        list[7].value = carType.prod_start;
                        list[8].value = carType.grade;
                        list[9].value = carType.atm_mtm;
                        list[10].value = carType.trans;
                        list[11].value = carType.f1;

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
        public static DetailsInNode GetDetailsInNode(string node_id, string lang = "EN", string brand_id = "TOYOTA")
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
                                    " pc.pic_code image_id, " +
                                    " pc.img_format ext, " +
                                   $" CONCAT('{brand_id}', '/', pc.catalog, '/', 'grimages', '/', pc.pic_code, pc.img_format ) path " +
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
        private static string GetWhereFromFilter(string[] param)
        {
            string strRes = "";
          //  string []  strKeys = { "model_name", "catalog", "model_code", "add_codes", "engine", "prod_start", "grade", "atm_mtm", "trans", "f1" };

            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("model_name", "model_name");
            dict.Add("catalog", "catalog");
            dict.Add("model_code", "model_code");
            dict.Add("add_codes", "add_codes");
            dict.Add("engine", "engine1"); //  не совпадает!
            dict.Add("prod_start", "prod_start");
            dict.Add("grade", "grade");
            dict.Add("atm_mtm", "atm_mtm");
            dict.Add("trans", "trans");
            dict.Add("f1", "f1");

            Array.Sort(param, StringComparer.InvariantCulture);
            if (param.Length > 0)
            {
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    string innerStr = string.Empty;
                    for (int i = 0; i < param.Length; i++)
                    {
                        string strTemp = param[i];

                        if (strTemp.IndexOf(pair.Key) > -1)
                        {
                            string strKey = pair.Key + "_";
                            if (String.IsNullOrEmpty(innerStr))
                            {
                                innerStr = " '" + strTemp.Substring(strTemp.IndexOf(strKey) + strKey.Length, strTemp.Length - (strTemp.IndexOf(strKey) + strKey.Length)) + "' ";
                            }
                            else
                            {
                                innerStr += ", '" + strTemp.Substring(strTemp.IndexOf(strKey) + strKey.Length, strTemp.Length - (strTemp.IndexOf(strKey) + strKey.Length)) + "' ";
                            }
                        }
                    }

                    if(!String.IsNullOrEmpty(innerStr))
                    {
                        strRes += $" AND mc.{pair.Value} IN ({innerStr}) ";
                    }
                 }
             }

            return strRes;
        }
    }
}
