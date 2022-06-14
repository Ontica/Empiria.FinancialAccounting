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


    protected override void Initialize() {
      Payload.Initialize(this.Type);
    }


    protected override string GetCommandTypeName() {
      return Type.ToString();
    }


    internal ReportRowFields MapToRowEditionField() {
      throw Assertion.EnsureNoReachThisCode("MapToRowEditionField");
    }


    public class PayloadType {

      public string ReportTypeUID {
        get; set;
      }

      public string RowUID {
        get; set;
      }

      public Positioning Positioning {
        get;
        internal set;
      }
      public string CellUID {
        get;
        set;
      }

      internal void Initialize(EditFinancialReportCommandType type) {

      }

    } // class PayloadType


    internal class EntitiesType {
      public FinancialReportType FinancialReportType {
        get;
        internal set;
      }
    }

  }  // class EditFinancialReportCommand

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
