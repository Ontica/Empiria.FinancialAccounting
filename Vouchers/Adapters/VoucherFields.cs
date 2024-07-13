/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Input Data Holder                       *
*  Type     : VoucherFields                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to create or update vouchers data.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Data structure used for update voucher concepts and also for protect voucher cloning.</summary>
  public class UpdateVoucherFields {

    public string Concept {
      get; set;
    } = string.Empty;


    public DateTime AccountingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public DateTime RecordingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;

  }  // UpdateVoucherFields


  /// <summary>Data structure that serves as an adapter to create or update vouchers data.</summary>
  public class VoucherFields {

    public string Concept {
      get; set;
    } = string.Empty;


    public DateTime AccountingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public DateTime RecordingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public int ElaboratedById {
      get; set;
    }

    public string LedgerUID {
      get; set;
    } = string.Empty;


    public string TransactionTypeUID {
      get; set;
    } = TransactionType.Manual.UID;


    public string VoucherTypeUID {
      get; set;
    } = string.Empty;


    public int FunctionalAreaId {
      get; set;
    } = -1;


    internal void EnsureValid() {
      if (AccountingDate.Date == new DateTime(2022, 1, 1)) {
        Assertion.Require(VoucherTypeUID == "c94bfd2b-84a7-4807-99fc-d4cb23cd43e3",
                         "El primero de enero de 2022 sólo permite el registro de pólizas de 'Carga de saldos iniciales'.");
      }

      if (AccountingDate.Date != new DateTime(2022, 1, 1) && VoucherTypeUID == "c94bfd2b-84a7-4807-99fc-d4cb23cd43e3") {
        Assertion.RequireFail("Las pólizas de 'Carga de saldos iniciales' deben registrarse con fecha primero de enero de 2022.");
      }


      if (AccountingDate.Date == new DateTime(2022, 1, 2)) {
        Assertion.Require(VoucherTypeUID == "e05e39ed-e744-43e1-b7b1-8e7b6c4e9895",
                         "El 2 de enero de 2022 sólo permite el registro de pólizas de 'Efectos iniciales de adopción de Norma'.");
      }

      if (AccountingDate.Date != new DateTime(2022, 1, 2) && VoucherTypeUID == "e05e39ed-e744-43e1-b7b1-8e7b6c4e9895") {
        Assertion.RequireFail("Las pólizas de 'Efectos iniciales de adopción de Norma' deben registrarse con fecha 2 de enero de 2022.");
      }

    }

  }  // class VoucherFields

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
