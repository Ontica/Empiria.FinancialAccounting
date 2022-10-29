/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Interface adapters                      *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Mapper class                            *
*  Type     : ExternalVariableMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial external variables.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.ExternalData.Adapters {

  /// <summary>Mapping methods for financial external variables.</summary>
  static public class ExternalVariableMapper {


    static internal FixedList<ExternalVariablesSetDto> Map(FixedList<ExternalVariablesSet> list) {
      return list.Select(x => Map(x)).ToFixedList();
    }


    static internal FixedList<ExternalVariableDto> Map(FixedList<ExternalVariable> list) {
      return list.Select(variable => Map(variable))
                 .ToFixedList();
    }


    static public ExternalVariableDto Map(ExternalVariable variable) {
      return new ExternalVariableDto {
        UID = variable.UID,
        Code = variable.Code,
        Name = variable.Name,
        Notes = variable.Notes,
        Position = variable.Position,
        SetUID = variable.Set.UID
      };
    }


    static private ExternalVariablesSetDto Map(ExternalVariablesSet set) {
      return new ExternalVariablesSetDto {
         UID = set.UID,
         Name = set.Name,
         ExportTo = ExportToMapper.Map(set.ExportTo)
      };
    }

  }  // class ExternalVariableMapper

}  // namespace Empiria.FinancialAccounting.ExternalData.Adapters
