/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Domain types                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information holder                   *
*  Type     : TransactionSlip                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds data about a transaction slip (volante).                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  /// <summary>Holds data about a transaction slip (volante).</summary>
  internal class TransactionSlip {

    #region Properties

    public string UID {
      get {
        if (this.Id != -1) {
          return this.Id.ToString();
        } else {
          return $"{SystemId}|{AccountingChartId}|{Number}|{AccountingDate.ToString("yyyy-MM-dd")}";
        }
      }
    }


    [DataField("ID_VOLANTE", ConvertFrom = typeof(long))]
    public long Id {
      get;
      internal set;
    }


    [DataField("ENC_TIPO_CONT")]
    public int AccountingChartId {
      get;
      internal set;
    }


    [DataField("ENC_NUM_VOL")]
    public long Number {
      get;
      internal set;
    }


    [DataField("ENC_SISTEMA", ConvertFrom = typeof(long))]
    public int SystemId {
      get;
      internal set;
    }


    [DataField("ENC_DESCRIP")]
    public string Concept {
      get;
      internal set;
    }


    [DataField("ENC_FECHA_VOL")]
    public DateTime AccountingDate {
      get;
      internal set;
    }


    [DataField("ENC_FECHA_CAP")]
    public DateTime RecordingDate {
      get;
      internal set;
    }


    [DataField("ENC_AREA_CAP")]
    public string FunctionalArea {
      get;
      internal set;
    }

    [DataField("ID_TRANSACCION", ConvertFrom = typeof(long))]
    public long AccountingVoucherId {
      get;
      internal set;
    }

    [DataField("ENC_USUARIO")]
    public string ElaboratedBy {
      get;
      internal set;
    }


    [DataField("ENC_TOT_CARGOS")]
    public decimal ControlTotal {
      get;
      internal set;
    }

    [DataField("STATUS", Default = TransactionSlipStatus.Pending)]
    public TransactionSlipStatus Status {
      get;
      internal set;
    }


    public string StatusName {
      get {

        switch (Status) {

          case TransactionSlipStatus.Pending:
            return "Pendiente";

          case TransactionSlipStatus.ProcessedWithIssues:
            return "Procesada con errores";

          case TransactionSlipStatus.ProcessedOK:
            return "Póliza generada";

          default:
            throw Assertion.AssertNoReachThisCode();

        }
      }
    }

    #endregion Properties

  }  // class TransactionSlip

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
