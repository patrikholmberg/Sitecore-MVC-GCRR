Sitecore-MVC-GCRR
=================

Global Conditional Rendering rules with Sitecore MVC

The story behind this project is that we wanted to have a global rule that checked if the rendering had a datasource and if that item which the datasource pointed to had any versions. If not the rendering shoulden't be displayed. 
This is where Conditional Renderings come in.

Conditional Rendering rules are used to manipulate the renderings based on certain criteria or conditions. Most often these rules are configured individually for the renderings.
However in some cases when it comes to rules that should affect several renderings it is easier to only have to configure it once and have it affect all renderings, namely Global Conditions.
Unfortunately Global Conditional Rendering Rules only work out of the box with WebForms and not MVC and had to be implemented.

Be warned though, having several global rules will affect the performance of the site!

In this repository you will find a processor for Sitecore MVC that enabled Global Conditional Rendering Rules and also condition (might show up more conditions and actions)