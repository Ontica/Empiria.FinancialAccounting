/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportItemDefinition              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Base class for report rows or cells.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Base class for report rows or cells.</summary>
  public abstract class FinancialReportItemDefinition : BaseObject {

    #region Constructors and parsers

    protected FinancialReportItemDefinition() {
      // Required by Empiria Framework.
    }

    protected FinancialReportItemDefinition(FinancialReportType reportType) {
      Assertion.Require(reportType, nameof(reportType));

      this.FinancialReportType = reportType;
    }

    static public FinancialReportItemDefinition Parse(int id) {
      return BaseObject.ParseId<FinancialReportItemDefinition>(id);
    }


    static public FinancialReportItemDefinition Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportItemDefinition>(uid);
    }


    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_REPORTE")]
    public FinancialReportType FinancialReportType {
      get; private set;
    }


    [DataField("ID_CONCEPTO")]
    public FinancialConcept FinancialConcept {
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


    [DataField("FILA")]
    public int RowIndex {
      get; private set;
    }


    [DataField("SECCION")]
    public string Section {
      get; private set;
    }


    [DataField("ELEMENTO_REPORTE_EXT_DATA")]
    internal protected JsonObject ExtendedData {
      get; set;
    } = JsonObject.Empty;


    [DataField("STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }

    static public FinancialReportItemDefinition Empty => BaseObject.ParseEmpty<FinancialReportItemDefinition>();

    #endregion Properties

    #region Methods

    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }


    internal void Update(ReportItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.FinancialConcept = fields.FinancialConcept;
      this.Label = fields.Label;
      this.Format = fields.Format;
      this.RowIndex = fields.Row;
    }

    #endregion Methods

  }  // class FinancialReportItemDefinition

} // namespace Empiria.FinancialAccounting.FinancialReports
