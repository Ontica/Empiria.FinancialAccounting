/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : ExternalVariableMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial external variables.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial external variables.</summary>
  static internal class ExternalVariableMapper {

    internal static FixedList<ExternalVariableDto> Map(FixedList<ExternalVariable> list) {
      return list.Select(variable => Map(variable))
                 .ToFixedList();
    }


    private static ExternalVariableDto Map(ExternalVariable variable) {
      return new ExternalVariableDto {
        UID = variable.UID,
        Code = variable.Code,
        Name = variable.Name,
        Notes = variable.Notes,
        Position = variable.Position,
        SetUID = variable.Set.UID
      };
    }

  }  // class ExternalVariableMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
