/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Action flags                            *
*  Type     : VoucherActions                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Action flags for accounting vouchers edition.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Action flags for accounting vouchers edition.</summary>
  public class VoucherActions {

    internal VoucherActions() {
      // no-op
    }

    public bool ChangeConcept {
      get; internal set;
    }

    public bool CloneVoucher {
      get; internal set;
    }

    public bool DeleteVoucher {
      get; internal set;
    }

    public bool EditVoucher {
      get; internal set;
    }

    public bool ReviewVoucher {
      get; internal set;
    }

    public bool SendToLedger {
      get; internal set;
    }

    public bool SendToSupervisor {
      get; internal set;
    }

  }  // VoucherActions

}  // namespace Empiria.FinancialAccounting.Vouchers
