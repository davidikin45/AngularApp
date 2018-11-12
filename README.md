# Getting Started with Angular and .NET Core

## VS Code Extensions
* [C# for Visual Studio Code (powered by OmniSharp)](https://github.com/OmniSharp/omnisharp-vscode)
* [C# Extensions](https://github.com/jchannon/csharpextensions)
* [Nuget Package Manager](https://github.com/jmrog/vscode-nuget-package-manager)
* Angular Snippets
* Angular Files
* Angular Language Service
* Angular-2 switcher (alt+o)
* Auto Rename Tag
* Bracket Pair Colorizer
* Debugger for Chrome
* Material Icon Theme
* Path Intellisense
* Prettier
* TSLint

## Creating a new Api and Spa Solution
1. Create a new directory such as DatingApp
2. Run the following command
```
npm install -g @angular/cli@latest
dotnet new webapi -o DatingApp.Api -n DatingApp.Api
dotnet new angular -o DatingApp.Spa
```
3. Add a .gitignore to the Api project
```
.gitignore
.vscode
bin
obj
*.db
```
4. Modify app.module.ts to import HttpClientModule 
```
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule
   ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
```
5. Install boostrap font-awesome
```
cd DatingApp.Spa\ClientApp
npm install boostrap font-awesome
```
6. Modify styles.css
```
@import '../node_modules/boostrap/dist/css/bootstrap.min.css';
@import '../node_modules/font-awesome/dist/css/font-awesome.min.css';
```

## Authors

* **David Ikin**