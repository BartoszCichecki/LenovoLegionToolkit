/* -----------------------------------------------------------------------------
 * tclsh.i
 *
 * SWIG File for building new tclsh program
 * ----------------------------------------------------------------------------- */

#ifdef AUTODOC
%subsection "tclsh.i"
%text %{
This module provides the Tcl_AppInit() function needed to build a 
new version of the tclsh executable.   This file should not be used
when using dynamic loading.   To make an interface file work with
both static and dynamic loading, put something like this in your
interface file :

     #ifdef STATIC
     %include <tclsh.i>
     #endif
%}
#endif

%{

/* A TCL_AppInit() function that lets you build a new copy
 * of tclsh.
 *
 * The macro SWIG_init contains the name of the initialization
 * function in the wrapper file.
 */

#ifndef SWIG_RcFileName
char *SWIG_RcFileName = "~/.myapprc";
#endif


int Tcl_AppInit(Tcl_Interp *interp){

  if (Tcl_Init(interp) == TCL_ERROR) 
    return TCL_ERROR;

  /* Now initialize our functions */

  if (SWIG_init(interp) == TCL_ERROR)
    return TCL_ERROR;
  Tcl_SetVar(interp, (char *) "tcl_rcFileName",SWIG_RcFileName,TCL_GLOBAL_ONLY);

  return TCL_OK;
}

int main(int argc, char **argv) {
  Tcl_Main(argc, argv, Tcl_AppInit);
  return(0);

}

%}

