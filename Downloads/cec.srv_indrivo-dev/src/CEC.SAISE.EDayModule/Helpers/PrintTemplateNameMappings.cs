using System.Collections.Generic;

namespace CEC.SAISE.EDayModule.Helpers
{
    public static class PrintTemplateNameMappings
    {
        public static Dictionary<int, string> GetMappings()
        {
            return new Dictionary<int, string>
            {
                { 6, "_ProcessVerbalBESVOnVoteCountResultsElectionsTemplate" },
                { 7, "_ProcessVerbalBESVOnVoteCountResultsReferendumTemplate" },
                { 8, "_ProcessVerbalBESVOnVoteCountResultsUTAGagauziaElectionsTemplate" },
                { 9, "_ProcessVerbalBESVOnVoteCountResultsUTAGagauziaReferendumTemplate" },
                { 10, "_FinalBESVReportElections1Day1stRoundTemplate" },
                { 11, "_FinalBESVReportElections1Day2ndRoundTemplate" },
                { 12, "_FinalBESVReportElections2Days1stRoundTemplate" },
                { 13, "_FinalBESVReportElections2Days2ndRoundTemplate" },
                { 14, "_FinalBESVReportReferendum1dayTemplate" },
                { 15, "_FinalBESVReportReferendum2daysTemplate" },
                { 16, "_FinalBESVReportElections1Day1stRoundUTAGTemplate" },
                { 17, "_FinalBESVReportElections1Day2ndRoundUTAGTemplate" },
                { 18, "_FinalBESVReportElections2Days1stRoundUTAGTemplate" },
                { 19, "_FinalBESVReportElections2Days2ndRoundUTAGTemplate" },
                { 20, "_FinalBESVReportReferendum1dayUTAGTemplate" },
                { 21, "_FinalBESVReportReferendum2daysUTAGTemplate" },
                { 24, "_ProcessVerbalCECECouncillorTemplate" },
                { 25, "_ProcessVerbalCECEMayorTemplate" },
                { 26, "_ProcessVerbalCECEParliamentRMTemplate" },
                { 27, "_ProcessVerbalCECEPresidentRMTemplate" },
                { 28, "_ProcessVerbalCECEReferendumTemplate" },
                { 29, "_ProcessVerbalCECEGovernorUTAGTemplate" },
                { 30, "_ProcessVerbalCECEDeputyAPUTAGTemplate" },
                { 31, "_ProcessVerbalCECEReferendumUTAGTemplate" },
                { 32, "_FinalCECEReportElections1Day1stRoundTemplate" },
                { 33, "_FinalCECEReportElections1Day2ndRoundTemplate" },
                { 34, "_FinalCECEReportElections2Days1stRoundTemplate" },
                { 35, "_FinalCECEReportElections2Days2ndRoundTemplate" },
                { 36, "_FinalCECEReportReferendum1dayTemplate" },
                { 37, "_FinalCECEReportReferendum2daysTemplate" },
                { 38, "_FinalCECEReportElections1Day1stRoundUTAGTemplate" },
                { 39, "_FinalCECEReportElections1Day2ndRoundUTAGTemplate" },
                { 40, "_FinalCECEReportElections2Days1stRoundUTAGTemplate" },
                { 41, "_FinalCECEReportElections2Days2ndRoundUTAGTemplate" },
                { 42, "_FinalCECEReportReferendum1dayUTAGTemplate" },
                { 43, "_FinalCECEReportReferendum2daysUTAGTemplate" },
            };
        }
    }
}