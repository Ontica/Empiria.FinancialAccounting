/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Mapper class                         *
*  Type     : TransactionSlipMapper                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Mapping methods for transaction slips (volantes).                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Mapping methods for transaction slips (volantes).</summary>
  static internal class TransactionSlipMapper {

    static internal FixedList<TransactionSlipDescriptorDto> Map(FixedList<TransactionSlip> list) {
      return new FixedList<TransactionSlipDescriptorDto>(list.Select(x => MapToDescriptor(x)));
    }


    static internal TransactionSlipDto Map(TransactionSlip transactionSlip) {
      return new TransactionSlipDto {
         Header = MapToDescriptor(transactionSlip),
         // Entries = transactionSlip.
      };
    }


    static private TransactionSlipDescriptorDto MapToDescriptor(TransactionSlip slip) {
      return new TransactionSlipDescriptorDto {
         UID = slip.UID,
         SlipNumber = slip.Number.ToString("00000"),
         Concept = EmpiriaString.Clean(slip.Concept),
         AccountingDate = slip.AccountingDate,
         RecordingDate = slip.RecordingDate,
         FunctionalArea = slip.FunctionalArea,
         AccountingVoucherId = slip.AccountingVoucherId,
         ElaboratedBy = slip.ElaboratedBy.Trim(),
         ControlTotal = slip.ControlTotal,
         StatusName = slip.StatusName
      };
    }

  }  // class TransactionSlipMapper

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
