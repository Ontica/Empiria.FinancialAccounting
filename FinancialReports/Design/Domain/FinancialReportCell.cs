/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportCell                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report fixed cell.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Describes a financial report fixed row.</summary>
  public class FinancialReportCell : BaseObject {

    #region Constructors and parsers

    protected FinancialReportCell() {
      // Required by Empiria Framework.
    }


    static public FinancialReportCell Parse(int id) {
      return BaseObject.ParseId<FinancialReportCell>(id);
    }


    static public FinancialReportCell Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportCell>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CONCEPTO")]
    public FinancialConcept FinancialConcept {
      get; private set;
    }

    [DataField("DATA_FIELD")]
    public string DataField {
      get; internal set;
    }

    [DataField("ETIQUETA")]
    public string Label {
      get; private set;
    }

    [DataField("FORMATO")]
    public string Format {
      get; private set;
    }

    [DataField("FILA")]
    public int Row {
      get; private set;
    }

    [DataField("COLUMNA")]
    public string Column {
      get; private set;
    }

    #endregion Properties

  }  // class FinancialReportCell

}  // namespace Empiria.FinancialAccounting.FinancialReports
