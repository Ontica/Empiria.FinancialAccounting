/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Command                                 *
*  Type     : EditFinancialReportCommand                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial report items.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Enumerates the command types used to create or update
  /// financial concept integration entries.</summary>
  public enum EditFinancialReportCommandType {

    InsertConceptCell,

    InsertConceptRow,

    InsertLabelCell,

    InsertLabelRow,

    UpdateConceptCell,

    UpdateConceptRow,

    UpdateLabelCell,

    UpdateLabelRow,

    Undefined

  }  // enum EditFinancialConceptEntryCommandType


  /// <summary>The command used to create or update financial report items.</summary>
  public class EditFinancialReportCommand : Command {

    public EditFinancialReportCommandType Type {
      get; set;
    } = EditFinancialReportCommandType.Undefined;


    public PayloadType Payload {
      get; set;
    } = new PayloadType();


    internal EntitiesType Entities {
      get; private set;
    } = new EntitiesType();


    protected override string GetCommandTypeName() {
      return Type.ToString();
    }


    protected override void Initialize() {
      Payload.Initialize(this.Type);
    }


    protected override void SetEntities() {
      Entities.FinancialReportType = FinancialReportType.Parse(Payload.ReportTypeUID);

      if (Payload.FinancialConceptUID.Length != 0) {
        Entities.FinancialConcept = FinancialConcept.Parse(Payload.FinancialConceptUID);
      }

      if (Payload.Positioning.Rule.UsesOffset()) {
        FinancialReportRow offsetRow = Entities.FinancialReportType.GetRow(Payload.Positioning.OffsetUID);

        Payload.Positioning.SetOffsetObject(offsetRow);
      }
    }

    internal ReportCellFields MapToReportCellFields() {
      return new ReportCellFields {
        FinancialConcept = this.Entities.FinancialConcept,
        Row = this.Payload.Row,
        Column = this.Payload.Column,
        DataField = this.Payload.DataField,
        Label = this.Payload.Label,
        Format = this.Payload.Format
      };
    }


    internal ReportRowFields MapToReportRowFields() {
      return new ReportRowFields {
        FinancialConcept = this.Entities.FinancialConcept,
        Row = this.Payload.Row,
        Label = this.Payload.Label,
        Format = this.Payload.Format
      };
    }


    public class PayloadType {

      public string ReportItemUID {
        get; set;
      } = string.Empty;


      public string ReportTypeUID {
        get; set;
      } = string.Empty;


      public string FinancialConceptUID {
        get; set;
      } = string.Empty;


      public string DataField {
        get; set;
      } = string.Empty;


      public int Row {
        get; set;
      }

      public string Column {
        get; set;
      } = string.Empty;


      public string Label {
        get; set;
      } = string.Empty;


      public string Format {
        get; set;
      } = string.Empty;


      public Positioning Positioning {
        get; internal set;
      } = new Positioning();


      internal void Initialize(EditFinancialReportCommandType type) {
        // no-op
      }


    } // inner class PayloadType


    internal class EntitiesType {

      public FinancialReportType FinancialReportType {
        get; internal set;
      }

      public FinancialConcept FinancialConcept {
        get; internal set;
      } =  FinancialConcept.Empty;


    }  // inner class EntitiesType

  }  // class EditFinancialReportCommand

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
