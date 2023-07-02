/* -----------------------------------------------------------------------------
 * phpinterfaces.i
 *
 * Define "known" PHP interfaces.
 *
 * These can be added at MINIT time (which is when PHP loads the extension
 * module).
 *
 * Any interface can be added via phpinterfaces, but looking up the
 * zend_class_entry by name has to wait until RINIT time, which means it
 * happens for every request.
 * ----------------------------------------------------------------------------- */

// Note: Abstract interfaces such as "Traversable" can't be used in
// "implements" so are not relevant here.

%insert(header) %{

#define SWIG_PHP_INTERFACE_Iterator_CE zend_ce_iterator
#define SWIG_PHP_INTERFACE_Iterator_HEADER "zend_interfaces.h"

#define SWIG_PHP_INTERFACE_IteratorAggregate_CE zend_ce_aggregate
#define SWIG_PHP_INTERFACE_IteratorAggregate_HEADER "zend_interfaces.h"

#define SWIG_PHP_INTERFACE_ArrayAccess_CE zend_ce_arrayaccess
#define SWIG_PHP_INTERFACE_ArrayAccess_HEADER "zend_interfaces.h"

#define SWIG_PHP_INTERFACE_Serializable_CE zend_ce_serializable
#define SWIG_PHP_INTERFACE_Serializable_HEADER "zend_interfaces.h"

#define SWIG_PHP_INTERFACE_Countable_CE zend_ce_countable
#define SWIG_PHP_INTERFACE_Countable_HEADER "zend_interfaces.h"

#define SWIG_PHP_INTERFACE_OuterIterator_CE spl_ce_OuterIterator
#define SWIG_PHP_INTERFACE_OuterIterator_HEADER "ext/spl/spl_iterators.h"

#define SWIG_PHP_INTERFACE_RecursiveIterator_CE spl_ce_RecursiveIterator
#define SWIG_PHP_INTERFACE_RecursiveIterator_HEADER "ext/spl/spl_iterators.h"

#define SWIG_PHP_INTERFACE_SeekableIterator_CE spl_ce_SeekableIterator
#define SWIG_PHP_INTERFACE_SeekableIterator_HEADER "ext/spl/spl_iterators.h"

#define SWIG_PHP_INTERFACE_SplObserver_CE spl_ce_SplObserver
#define SWIG_PHP_INTERFACE_SplObserver_HEADER "ext/spl/spl_observer.h"

#define SWIG_PHP_INTERFACE_SplSubject_CE spl_ce_SplSubject
#define SWIG_PHP_INTERFACE_SplSubject_HEADER "ext/spl/spl_observer.h"

#define SWIG_PHP_INTERFACE_DateTimeInterface_CE php_date_get_interface_ce()
#define SWIG_PHP_INTERFACE_DateTimeInterface_HEADER "ext/date/php_date.h"

// The "json" extension needs to be loaded earlier that us for this to work.
#define SWIG_PHP_INTERFACE_JsonSerializable_CE php_json_serializable_ce
#define SWIG_PHP_INTERFACE_JsonSerializable_HEADER "ext/json/php_json.h"

// New in PHP 8.0.
#if PHP_MAJOR_VERSION >= 8
# define SWIG_PHP_INTERFACE_Stringable_CE zend_ce_stringable
# define SWIG_PHP_INTERFACE_Stringable_HEADER "zend_interfaces.h"
#endif

%}
