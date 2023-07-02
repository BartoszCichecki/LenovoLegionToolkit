/* -----------------------------------------------------------------------------
 * wish.i
 *
 * SWIG File for making wish
 * ----------------------------------------------------------------------------- */

#ifdef AUTODOC
%subsection "wish.i"
%text %{
This module provides the Tk_AppInit() function needed to build a 
new version of the wish executable.   Like tclsh.i, this file should
not be used with dynamic loading.  To make an interface file work with
both static and dynamic loading, put something like this in your
interface file :

     #ifdef STATIC
     %include <wish.i>
     #endif

A startup file may be specified by defining the symbol SWIG_RcFileName
as follows (this should be included in a code-block) :

     #define SWIG_RcFileName    "~/.mywishrc"
%}
#endif

%{


/* Initialization code for wish */

#include <tk.h>

#ifndef SWIG_RcFileName
char *SWIG_RcFileName = "~/.wishrc";
#endif

/*
 *----------------------------------------------------------------------
 *
 * Tcl_AppInit --
 *
 *	This procedure performs application-specific initialization.
 *	Most applications, especially those that incorporate additional
 *	packages, will have their own version of this procedure.
 *
 * Results:
 *	Returns a standard Tcl completion code, and leaves an error
 *	message in interp->result if an error occurs.
 *
 * Side effects:
 *	Depends on the startup script.
 *
 *----------------------------------------------------------------------
 */

int Tcl_AppInit(Tcl_Interp *interp)
{
    Tk_Window main;
    main = Tk_MainWindow(interp);

    /*
     * Call the init procedures for included packages.  Each call should
     * look like this:
     *
     * if (Mod_Init(interp) == TCL_ERROR) {
     *     return TCL_ERROR;
     * }
     *
     * where "Mod" is the name of the module.
     */

    if (Tcl_Init(interp) == TCL_ERROR) {
	return TCL_ERROR;
    }

    if (Tk_Init(interp) == TCL_ERROR) {
	return TCL_ERROR;
    }

    /*
     * Call Tcl_CreateCommand for application-specific commands, if
     * they weren't already created by the init procedures called above.
     */

    if (SWIG_init(interp) == TCL_ERROR) {
      return TCL_ERROR;
    }
    
    /*
     * Specify a user-specific startup file to invoke if the application
     * is run interactively.  Typically the startup file is "~/.apprc"
     * where "app" is the name of the application.  If this line is deleted
     * then no user-specific startup file will be run under any conditions.
     */

    Tcl_SetVar(interp, (char *) "tcl_rcFileName",SWIG_RcFileName,TCL_GLOBAL_ONLY);
    return TCL_OK;
}

#if TK_MAJOR_VERSION >= 4
int main(int argc, char **argv) {
  Tk_Main(argc, argv, Tcl_AppInit);
  return(0);
}
#else
extern int main();
#endif

%}



