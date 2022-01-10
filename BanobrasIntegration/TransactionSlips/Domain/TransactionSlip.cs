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

    #region Constructors and parsers

    static public TransactionSlip Parse(string uid) {
      Assertion.AssertObject(uid, "uid");

      if (uid.Contains("~")) {
        return TransactionSlipData.GetPendingTransactionSlip(uid);
      } else {
        return TransactionSlipData.GetProcessedTransactionSlip(uid);
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ENC_UID")]
    public string UID {
      get;
      private set;
    }


    [DataField("ID_VOLANTE", ConvertFrom = typeof(long))]
    public long Id {
      get;
      private set;
    }


    [DataField("ENC_TIPO_CONT")]
    public int AccountingChartId {
      get;
      private set;
    }


    [DataField("ENC_NUM_VOL")]
    public long Number {
      get;
      private set;
    }


    [DataField("ENC_SISTEMA", ConvertFrom = typeof(long))]
    public int SystemId {
      get;
      private set;
    }


    [DataField("ENC_DESCRIP")]
    public string Concept {
      get;
      private set;
    }


    [DataField("ENC_FECHA_VOL")]
    public DateTime AccountingDate {
      get;
      private set;
    }


    [DataField("ENC_FECHA_CAP")]
    public DateTime RecordingDate {
      get;
      private set;
    }


    [DataField("ENC_AREA_CAP")]
    public string FunctionalArea {
      get;
      private set;
    }


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(long))]
    public long AccountingVoucherId {
      get;
      private set;
    }


    [DataField("ENC_USUARIO")]
    public string ElaboratedBy {
      get;
      private set;
    }


    [DataField("ENC_TOT_CARGOS")]
    public decimal ControlTotal {
      get;
      private set;
    }


    [DataField("STATUS", Default = TransactionSlipStatus.Pending)]
    public TransactionSlipStatus Status {
      get;
      private set;
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
