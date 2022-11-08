/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Query payload                        *
*  Type     : ReportBuilderQuery                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Query payload used to generate financial accounting reports.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Query payload used to generate financial accounting reports.</summary>
  public class ReportBuilderQuery {

    public ReportTypes ReportType {
      get; set;
    }

    public FileType ExportTo {
      get; set;
    } = FileType.Xml;


    public string AccountsChartUID {
      get; set;
    }

    public DateTime FromDate {
      get; set;
    } = DateTime.Now;

    public DateTime ToDate {
      get; set;
    }

    public string[] Ledgers {
      get; set;
    } = new string[0];


    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public string OutputType {
      get; set;
    } = string.Empty;


    public SendType SendType {
      get; set;
    } = SendType.N;


    public bool WithSubledgerAccount {
      get; set;
    } = false;

  } // class ReportBuilderQuery


  public enum ReportTypes {

    BalanzaSAT,

    BalanzaDeterminarImpuestos,

    CatalogoCuentasSAT,

    ListadoDePolizas,

    ListadoDePolizasPorCuenta,

    ComparativoDeCuentas

  } // enum ReportTypes


  public enum SendType {

    N,

    C

  } // enum SendType

} // namespace Empiria.FinancialAccounting.Reporting
