/* -----------------------------------------------------------------------------
 * javascriptruntime.swg
 *
 * ----------------------------------------------------------------------------- */

// V8 Version Macro
// ----------------
//
// v8 added version macros V8_MAJOR_VERSION, V8_MINOR_VERSION, V8_BUILD_NUMBER
// and V8_PATCH_LEVEL in version 4.3.0.  SWIG doesn't support anything that
// old so SWIG generated code can rely on these.

// Node support
// ------------

#ifdef BUILDING_NODE_EXTENSION
%insert("runtime") %{
#include <node.h>
//Older version of node.h does not include this
#include <node_version.h>
%}
#endif


// V8 runtime
// ----------

%insert(runtime) %{
#include <v8.h>

#undef SWIG_V8_VERSION
#define SWIG_V8_VERSION ((V8_MAJOR_VERSION / 10) * 4096 + \
                         (V8_MAJOR_VERSION % 10) * 256 + \
                         (V8_MINOR_VERSION / 10) * 16 + \
                         (V8_MINOR_VERSION % 10))

#include <errno.h>
#include <limits.h>
#include <stdlib.h>
#include <assert.h>
%}

%insert(runtime) "swigrun.swg";         /* SWIG API */
%insert(runtime) "swigerrors.swg";      /* SWIG errors */

%insert(runtime) "javascriptrun.swg"

