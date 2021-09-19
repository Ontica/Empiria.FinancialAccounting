/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportRow                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report fixed row.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Describes a financial report fixed row.</summary>
  public class FinancialReportRow: BaseObject {

    #region Constructors and parsers

    protected FinancialReportRow() {
      // Required by Empiria Framework.
    }


    static public FinancialReportRow Parse(int id) {
      return BaseObject.ParseId<FinancialReportRow>(id);
    }


    static public FinancialReportRow Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportRow>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CONCEPTO")]
    public GroupingRule GroupingRule {
      get; private set;
    }

    [DataField("CLAVE_CONCEPTO")]
    public string Code {
      get; private set;
    }

    [DataField("ETIQUETA")]
    public string Label {
      get; private set;
    }

    [DataField("FORMATO")]
    public string Format {
      get; private set;
    }

    [DataField("POSICION")]
    public int Position {
      get; private set;
    }


    #endregion Properties

  }  // class FinancialReportRow

}  // namespace Empiria.FinancialAccounting.FinancialReports
