# ExxerRules Installation Script
# This script runs when the package is installed

param($installPath, $toolsPath, $package, $project)

Write-Host "🎉 ExxerRules - Modern Development Conventions installed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 What's included:" -ForegroundColor Cyan
Write-Host "   • 20 production-ready Roslyn analyzers" -ForegroundColor White
Write-Host "   • Result<T> pattern enforcement" -ForegroundColor White
Write-Host "   • Clean Architecture validation" -ForegroundColor White
Write-Host "   • Modern testing standards" -ForegroundColor White
Write-Host "   • Async best practices" -ForegroundColor White
Write-Host "   • Performance optimization guidance" -ForegroundColor White
Write-Host ""
Write-Host "⚙️  Configuration:" -ForegroundColor Cyan
Write-Host "   Add to your .csproj to customize:" -ForegroundColor White
Write-Host ""
Write-Host "   <PropertyGroup>" -ForegroundColor Gray
Write-Host "     <ExxerRulesEnabled>true</ExxerRulesEnabled>" -ForegroundColor Gray
Write-Host "     <ExxerRulesSeverity>warning</ExxerRulesSeverity>" -ForegroundColor Gray
Write-Host "     <ExxerRulesArchitecture>true</ExxerRulesArchitecture>" -ForegroundColor Gray
Write-Host "     <ExxerRulesAsync>true</ExxerRulesAsync>" -ForegroundColor Gray
Write-Host "     <ExxerRulesErrorHandling>true</ExxerRulesErrorHandling>" -ForegroundColor Gray
Write-Host "     <ExxerRulesTesting>true</ExxerRulesTesting>" -ForegroundColor Gray
Write-Host "   </PropertyGroup>" -ForegroundColor Gray
Write-Host ""
Write-Host "📚 Documentation: https://github.com/exxerai/exxer-rules" -ForegroundColor Cyan
Write-Host "🐛 Issues: https://github.com/exxerai/exxer-rules/issues" -ForegroundColor Cyan
Write-Host ""
Write-Host "Happy coding with Modern Development Conventions! 🚀" -ForegroundColor Green
